/*
    Problem: Object Creation Logic Scattered Everywhere

    Without a factory, we end up duplicating the same if-else chain for creating payment processors
    in multiple places across our codebase.

    Issues:
    1. Duplicate code - same creation logic in CheckoutService, SubscriptionService, RefundService
    2. Hard to maintain - adding PayPal means updating 5+ different classes
    3. Violates DRY principle
    4. Testing is harder - can't mock processor creation easily

    This demonstrates why we need Simple Factory to centralize creation logic.
*/

public interface IPaymentProcessor
{
    PaymentResult ProcessPayment(decimal amount, PaymentDetails details);
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; }
    public string Message { get; set; }
}

public class PaymentDetails
{
    public string CustomerId { get; set; }
}

public class CreditCardProcessor : IPaymentProcessor
{
    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        return new PaymentResult { Success = true, TransactionId = $"CC_{Guid.NewGuid()}" };
    }
}

public class BKashProcessor : IPaymentProcessor
{
    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        return new PaymentResult { Success = true, TransactionId = $"BK_{Guid.NewGuid()}" };
    }
}

public class BankTransferProcessor : IPaymentProcessor
{
    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        return new PaymentResult { Success = true, TransactionId = $"BT_{Guid.NewGuid()}" };
    }
}

// Problem: Creation logic duplicated everywhere

public class CheckoutService
{
    public void ProcessCheckout(string paymentType, decimal amount, PaymentDetails details)
    {
        IPaymentProcessor processor;
        
        // Same if-else chain here...
        if (paymentType == "CreditCard")
            processor = new CreditCardProcessor();
        else if (paymentType == "BKash")
            processor = new BKashProcessor();
        else if (paymentType == "BankTransfer")
            processor = new BankTransferProcessor();
        else
            throw new Exception("Invalid payment type");
        
        var result = processor.ProcessPayment(amount, details);
    }
}

public class SubscriptionService
{
    public void CreateSubscription(string paymentType, decimal amount, PaymentDetails details)
    {
        IPaymentProcessor processor;
        
        // ...duplicated here...
        if (paymentType == "CreditCard")
            processor = new CreditCardProcessor();
        else if (paymentType == "BKash")
            processor = new BKashProcessor();
        else if (paymentType == "BankTransfer")
            processor = new BankTransferProcessor();
        else
            throw new Exception("Invalid payment type");
        
        var result = processor.ProcessPayment(amount, details);
    }
}

public class RefundService
{
    public void ProcessRefund(string paymentType, decimal amount, PaymentDetails details)
    {
        IPaymentProcessor processor;
        
        // ...and here...
        if (paymentType == "CreditCard")
            processor = new CreditCardProcessor();
        else if (paymentType == "BKash")
            processor = new BKashProcessor();
        else if (paymentType == "BankTransfer")
            processor = new BankTransferProcessor();
        else
            throw new Exception("Invalid payment type");
        
        var result = processor.ProcessPayment(amount, details);
    }
}

public class CartPreviewService
{
    public void CalculateFees(string paymentType)
    {
        IPaymentProcessor processor;
        
        // ...and here too!
        if (paymentType == "CreditCard")
            processor = new CreditCardProcessor();
        else if (paymentType == "BKash")
            processor = new BKashProcessor();
        else if (paymentType == "BankTransfer")
            processor = new BankTransferProcessor();
        else
            throw new Exception("Invalid payment type");
    }
}

/*
    What happens when we add PayPal?
    
    We need to:
    1. Modify CheckoutService
    2. Modify SubscriptionService
    3. Modify RefundService
    4. Modify CartPreviewService
    5. Hunt down any other place with this pattern
    
    This doesn't scale. We need a centralized factory.
*/
