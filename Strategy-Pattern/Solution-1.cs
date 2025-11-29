/*

    1. If I have to add "Same-Day" shipping, I need to modify the order class, which will violate the Open/Closed Principle.
    2. It is difficult to test the International shipping logic in isolation, because it is tightly coupled with the Order class.
    3. If Express shipping logic changes based on the carrier, I would have to keep modifying the Order class, I have to add nested if-else conditions which will make the code messy and hard to maintain. It will also violate the SRP, OCP principles.

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

public class CheckoutProcessor
{
    public void Execute()
    {
        Console.WriteLine("Enter shipping type (Standard, Express, International): ");
        string shippingType = Console.ReadLine();

        Console.WriteLine("Enter weight: ");
        decimal weightInput = decimal.Parse(Console.ReadLine());

        Console.WriteLine("Enter order value: ");
        decimal orderValueInput = decimal.Parse(Console.ReadLine());

        IShippingStrategy shippingStrategy;

        if (shippingType == "Standard")
        {
            shippingStrategy = new StandardShipping();
        }
        else if(shippingType == "Express")
        {
            shippingStrategy = new ExpressShipping();
        }
        else if(shippingType == "International")
        {
            shippingStrategy = new InternationalShipping();
        }
        else
        {
            throw new Exception("Invalid shipping type");
        }

        Order order = new Order(shippingStrategy);
        decimal shippingCost = order.CalculateShipping(weightInput, orderValueInput);
        Console.WriteLine($"Shipping Cost: {shippingCost}");
    }
}