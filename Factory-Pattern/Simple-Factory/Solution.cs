/*

    Use Simple Factory when:

    - We have a straightforward mapping: string → object
    - All objects share a common interface
    - Object creation logic is centralized
    - We don't need polymorphic factories
*/

public interface IPaymentProcessor
{
    PaymentResult ProcessPayment(decimal amount, PaymentDetails details);
    bool SupportsRefunds {get;}
}

public class PaymentResult
{
    public bool Success {get; set;}
    public string TransactionId {get; set;}
    public string Message {get; set;}
    public decimal Fee {get; set;}
}

public class PaymentDetails
{
    public string CustomerId {get; set;}
    public Dictionary<string, string> Metadata {get; set;}
}

public class CreditCardProcessor : IPaymentProcessor
{
    public bool SupportsRefunds => true;

    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        Console.WriteLine($"[CreditCard] Processing payment of {amount} for customer {details.CustomerId}");

        // Simulate processing logic
        var fee = amount * 0.03m; // 3% fee

        return new PaymentResult
        {
            Success = true,
            TransactionId = $"CC_{Guid.NewGuid()}",
            Message = "Payment processed successfully via Credit Card.",
            Fee = fee
        };
    }
}

public class BKashProcessor : IPaymentProcessor
{
    public bool SupportsRefunds => true;

    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        Console.WriteLine($"[BKash] Processing payment of {amount} for customer {details.CustomerId}");

        // Simulate processing logic
        var fee = 0m;

        return new PaymentResult
        {
            Success = true,
            TransactionId = $"BK_{Guid.NewGuid()}",
            Message = "Payment processed successfully via BKash.",
            Fee = fee
        };
    }
}

public class BankTransferProcessor : IPaymentProcessor
{
    public bool SupportsRefunds => false;

    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        Console.WriteLine($"[BankTransfer] Processing payment of {amount} for customer {details.CustomerId}");

        // Simulate processing logic
        var fee = 0.50m + 2;

        return new PaymentResult
        {
            Success = true,
            TransactionId = $"BT_{Guid.NewGuid()}",
            Message = "Payment processed successfully via Bank Transfer.",
            Fee = fee
        };
    }
}

public class PaymentProcessorFactory
{
    private readonly Dictionary<string, IPaymentProcessor> _processors;

    public PaymentProcessorFactory()
    {
        _processors = new Dictionary<string, IPaymentProcessor>(StringComparer.OrdinalIgnoreCase)
        {
            { "CreditCard", new CreditCardProcessor() },
            { "BKash", new BKashProcessor() },
            { "BankTransfer", new BankTransferProcessor() }
        };
    }

    public IPaymentProcessor GetPaymentProcessor(string paymentType)
    {
        if(_processors.TryGetValue(paymentType, out var processor))
        {
            return processor;
        }

        throw new ArgumentException($"Invalid payment type: {paymentType}", nameof(paymentType));
    }
}

public class CheckoutService
{
    private readonly PaymentProcessorFactory _factory;

    public CheckoutService(PaymentProcessorFactory factory)
    {
        _factory = factory;
    }

    public void ProcessCheckout(string paymentType, decimal amount, PaymentDetails details)
    {
        try
        {
            var processor = _factory.GetPaymentProcessor(paymentType);

            var result = processor.ProcessPayment(amount, details);

            if (result.Success)
            {
                Console.WriteLine($"Payment Successful! Transaction ID: {result.TransactionId}, Fee: {result.Fee}");
            }
        }
        
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}