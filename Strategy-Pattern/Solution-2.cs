/*
    So far now in Solution-1.cs, we have implemented the Strategy Pattern to handle different shipping strategies.
    The CheckoutProcessor code interacts with the Order class, which uses different shipping strategies based on user input. 
    This demonstrates the Strategy Pattern by encapsulating shipping algorithms in separate classes, allowing for easy extension and maintenance.

    But the if-else condition checking the shipping type is still present in the CheckoutProcessor class. It just moved from the Order class to the CheckoutProcessor class.
    To further improve this, we can use a Factory Pattern to create the appropriate shipping strategy based on the shipping type.
    This way, the CheckoutProcessor class does not need to know about the specific shipping strategy classes, and we can eliminate the if-else conditions.
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
    public decimal CalculateShipping(decimal weight, decimal orderValue)
    {
        return 15m + (2m * weight);
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
    public IShippingStrategy GetShippingStrategy(string shippingType)
    {
        return shippingType?.ToLower() switch
        {
            "standard" => new StandardShipping(),
            "express" => new ExpressShipping(),
            "international" => new InternationalShipping(),
            "sameday" => new SameDayShipping(),
            "overnight" => new OvernightShipping(),
            _ => throw new ArgumentException($"Invalid shipping type: {shippingType}", nameof(shippingType))
        };
    }
}

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
        var factory = new ShippingStrategyFactory();
        var processor = new CheckoutProcessor(factory);
        processor.Execute();
    }
}