/*
    What's Wrong With This?
    Think about these questions:

        1. What happens when you add "Same-Day" shipping next month?
        2. How would you test the International shipping logic in isolation?
        3. What if Express shipping rules change based on the carrier (FedEx vs UPS)?

    The Order class knows too much. Every time shipping logic changes, you modify this class. It violates the Open/Closed Principle (open for extension, closed for modification).
*/

public class Order
{
    public decimal CalculateShipping(string shippingType, decimal weight, decimal orderValue)
    {
        if (shippingType == "Standard")
        {
            return 5m;
        }
        else if (shippingType == "Express")
        {
            return 15m + (2m * weight);
        }
        else if (shippingType == "International")
        {
            return 30m + (5m * weight) + (orderValue * 0.1m);
        }
        return 0m;
    }
}