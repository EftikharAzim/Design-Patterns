/*
    In Solution-2.cs, we improved the Strategy Pattern implementation by introducing a Factory Pattern to create shipping strategies.
    This eliminated the if-else conditions in the CheckoutProcessor class, adhering to the OCP.

    Right now in the implementation, all strateties are stateless and do not depend on any external services.
    Now in Solution-4.cs, we consider a situation where ExpressShipping needs to call an external API to get carrier rates.
    To handle this, ExpressShipping will depend on an ICarrierApiClient interface.
    The factory will be responsible for injecting the dependency when creating the ExpressShipping strategy.
*/
public interface IShippingStrategy
{
    decimal CalculateShipping(decimal weight, decimal orderValue);
}

public class StandardShipping : IShippingStrategy
{
    public decimal CalculateShipping(decimal weight, decimal orderValue)
    {
        return 5m;
    }
}

public class ExpressShipping : IShippingStrategy
{
    // ICarrierApiClient is a hypothetical interface representing an external carrier API client.
    private readonly ICarrierApiClient _carrierApiClient;

    public ExpressShipping(ICarrierApiClient carrierApiClient)
    {
        _carrierApiClient = carrierApiClient ?? throw new ArgumentNullException(nameof(carrierApiClient));
    }
    public decimal CalculateShipping(decimal weight, decimal orderValue)
    {
        var carrierRate = _carrierApiClient.GetExpressRate(weight);
        return carrierRate.BaseCost + (carrierRate.PerKgCost * weight);
    }
}

public class InternationalShipping : IShippingStrategy
{
    public decimal CalculateShipping(decimal weight, decimal orderValue)
    {
        return 30m + (5m * weight) + (orderValue * 0.1m);
    }
}

public class SameDayShipping : IShippingStrategy
{
    public decimal CalculateShipping(decimal weight, decimal orderValue)
    {
        if (weight >= 10)
        {
            throw new InvalidOperationException("Same-day shipping is not available for orders 10kg or heavier");
        }
        return 25m;
    }
}

public class OvernightShipping : IShippingStrategy
{
    public decimal CalculateShipping(decimal weight, decimal orderValue)
    {
        if (weight >= 5)
        {
            throw new InvalidOperationException("Overnight shipping is not available for orders 5kg or heavier");
        }
        return 20m;
    }
}

public interface ICarrierApiClient
{
    CarrierRate GetExpressRate(decimal weight);
}

public class CarrierRate
{
    public decimal BaseCost {get; set;}
    public decimal PerKgCost {get; set;}
}

public class CarrierApiClient : ICarrierApiClient
{
    public CarrierRate GetExpressRate(decimal weight)
    {
        // In real implementation, this would call an external API.
        Console.WriteLine($"Fetching express rate for {weight}kg");
        System.Threading.Thread.Sleep(500); // Simulate network delay

        return new CarrierRate
        {
            BaseCost = 15m,
            PerKgCost = 2m
        };
    }
}

public class Order
{
    private readonly IShippingStrategy _shippingStrategy;

    public Order(IShippingStrategy shippingStrategy)
    {
        _shippingStrategy = shippingStrategy;
    }

    public decimal CalculateShipping(decimal weight, decimal orderValue)
    {
        return _shippingStrategy.CalculateShipping(weight, orderValue);
    }
}

public class ShippingStrategyFactory
{
    private readonly ICarrierApiClient _carrierApiClient;
    private readonly Dictionary<string, IShippingStrategy> _strategyCache;

    //Create strategies once and reuse them
    public ShippingStrategyFactory(ICarrierApiClient carrierApiClient)
    {
        _carrierApiClient = carrierApiClient ?? throw new ArgumentNullException(nameof(carrierApiClient));

        _strategyCache = new Dictionary<string, IShippingStrategy>(StringComparer.OrdinalIgnoreCase)
        {
            { "standard", new StandardShipping() },
            { "express", new ExpressShipping(_carrierApiClient) },
            { "international", new InternationalShipping() },
            { "sameday", new SameDayShipping() },
            { "overnight", new OvernightShipping() }   
        };
    }

    public IShippingStrategy GetShippingStrategy(string shippingType)
    {
        if(_strategyCache.TryGetValue(shippingType, out var strategy))
        {
            return strategy;
        }

        throw new ArgumentException($"Invalid shipping type: {shippingType}", nameof(shippingType));
    }
}

/*
    What If Multiple Strategies Need Different Dependencies?
    Right now only ExpressShipping needs ICarrierApiClient. But what if:

    InternationalShipping needs ICustomsCalculator
    SameDayShipping needs IWarehouseLocator

    This is fine for 3-5 dependencies, but at scale it gets unwieldy. In production, you'd use a DI container (like .NET's built-in IServiceProvider):

public class ShippingStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ShippingStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IShippingStrategy GetShippingStrategy(string shippingType)
    {
        return shippingType?.ToLower() switch
        {
            "standard" => new StandardShipping(),
            "express" => new ExpressShipping(
                _serviceProvider.GetRequiredService<ICarrierApiClient>()
            ),
            "international" => new InternationalShipping(
                _serviceProvider.GetRequiredService<ICustomsCalculator>()
            ),
            // ...
        };
    }
}
*/

public class CheckoutProcessor
{
    private readonly ShippingStrategyFactory _factory;

    public CheckoutProcessor(ShippingStrategyFactory factory)
    {
        _factory = factory;
    }

    public void Execute()
    {
        Console.WriteLine("Enter shipping type (Standard, Express, International, SameDay, Overnight): ");
        string shippingType = Console.ReadLine();

        Console.WriteLine("Enter weight (kg): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal weight) || weight < 0)
        {
            Console.WriteLine("Invalid weight");
            return;
        }

        Console.WriteLine("Enter order value: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal orderValue) || orderValue < 0)
        {
            Console.WriteLine("Invalid order value");
            return;
        }

        try
        {
            IShippingStrategy strategy = _factory.GetShippingStrategy(shippingType);
            Order order = new Order(strategy);
            decimal shippingCost = order.CalculateShipping(weight, orderValue);
            
            Console.WriteLine($"Shipping Cost: ${shippingCost:F2}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

public class Program
{
    public static void Main()
    {
        var carrierApiClient = new CarrierApiClient(); // Hypothetical implementation
        var factory = new ShippingStrategyFactory(carrierApiClient);
        var processor = new CheckoutProcessor(factory);
        processor.Execute();
    }
}