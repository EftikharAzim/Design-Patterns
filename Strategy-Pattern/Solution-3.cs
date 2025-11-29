/*
    So far in solution-2.cs, we have implemented the Factory Pattern along with the Strategy Pattern to handle different shipping strategies.
    The CheckoutProcessor code interacts with the Order class, which uses different shipping strategies based on user input.
    This demonstrates the Strategy Pattern by encapsulating shipping algorithms in separate classes, allowing for easy extension and maintenance.
    The Factory Pattern is used to create the appropriate shipping strategy based on the shipping type, eliminating the if-else conditions in the CheckoutProcessor class.
    This way, the CheckoutProcessor class does not need to know about the specific shipping strategy classes.

    But, what about some strategies need weight but other's don't? For example, Standard shipping doesn't need weight, but Express and International do.
    In the current implementation, all strategies have to accept both weight and order value parameters, even if they don't use them.
    To address this, we have two solutions.
        1. Parameter object
        2. Separate Strategy Interfaces like IWeightBasedShippingStrategy and IOrderValueBasedShippingStrategy

    In this solution-3.cs, we will implement the second approach by creating separate strategy interfaces for weight-based and order-value-based shipping strategies.
    Pros:
        1. Each strategy interface is focused on a specific type of shipping calculation, adhering to the Interface Segregation Principle.
        2. Strategies only implement the methods they need, reducing unnecessary parameters and improving clarity.
        3. Easier to test individual strategies in isolation.
    Cons:
        1. Slightly more complex design with multiple interfaces.
        2. Caller needs to know which properties to set based on the strategy being used.

    What's happening here:

        1. is keyword checks if the strategy implements a specific interface
        2. If it does, cast it and set the property
        3. Then call CalculateShipping() with no parameters

        This works, but notice the awkwardness: Order still needs to know about IWeightBasedShippingStrategy and IOrderValueBasedShippingStrategy. We've reduced coupling but haven't eliminated it.

    After implementing the above approach, ask yourself: What did we actually gain?

        1. Strategies still receive the same data
        2. Order class is now more complex with type checking
        3. Testing isn't significantly easier
        4. We've added cognitive overhead

    For this specific problem, the original interface is better:
    public interface IShippingStrategy
    {
        decimal CalculateShipping(decimal weight, decimal orderValue);
    }
    Why?

    It's a calculation, not stateful behavior — shipping cost is a pure function of inputs
    Two parameters isn't unwieldy — it's clear what the method needs
    Unused parameters are acceptable — StandardShipping ignoring weight doesn't hurt anything
    Simpler for the caller — no type checking needed

    Good use case (different scenario):
    Imagine we're building a notification system:
    // BAD: Fat interface
    public interface INotificationService
    {
        void SendEmail(string to, string subject, string body);
        void SendSms(string phoneNumber, string message);
        void SendPushNotification(string deviceToken, string title, string body);
        void SendSlackMessage(string channel, string message);
    }
    Now every implementation must implement all 4 methods, even if an EmailNotificationService doesn't send SMS.
*/

public interface IShippingStrategy
{
    decimal CalculateShipping();
}

public interface IWeightBasedShippingStrategy : IShippingStrategy
{
    decimal Weight {get; set;}
}

public interface IOrderValueBasedShippingStrategy : IShippingStrategy
{
    decimal OrderValue {get; set;}
}

public class StandardShipping : IShippingStrategy
{
    public decimal CalculateShipping() => 5m;
}

public class ExpressShipping : IWeightBasedShippingStrategy
{
    public decimal Weight {get; set;}
    public decimal CalculateShipping()
    {
        return 15m + (2m * Weight);
    }
}

public class InternationalShipping : IWeightBasedShippingStrategy, IOrderValueBasedShippingStrategy
{
    public decimal Weight {get; set;}
    public decimal OrderValue {get; set;}
    public decimal CalculateShipping()
    {
        return 30m + (5m * Weight) + (OrderValue * 0.1m);
    }
}

public class SameDayShipping : IWeightBasedShippingStrategy
{
    public decimal Weight {get; set;}
    public decimal CalculateShipping()
    {
        if(Weight >= 10)
        {
            throw new InvalidOperationException("Same Day shipping is not available for weight 10 or more.");
        }
        return 25m;
    }
}

public class OvernightShipping : IWeightBasedShippingStrategy
{
    public decimal Weight {get; set;}
    public decimal CalculateShipping()
    {
        if(Weight >= 5)
        {
            throw new InvalidOperationException("Overnight shipping is not available for weight 5 or more.");
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
        if(_shippingStrategy is IWeightBasedShippingStrategy weightBased)
        {
            weightBased.Weight = weight;
        }
        if(_shippingStrategy is IOrderValueBasedShippingStrategy orderValueBased)
        {
            orderValueBased.OrderValue = orderValue;
        }

        return _shippingStrategy.CalculateShipping();
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