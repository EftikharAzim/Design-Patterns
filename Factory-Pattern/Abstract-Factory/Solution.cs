/*
    Abstract Factory Pattern Example: Payment Processing System

    This example demonstrates the Abstract Factory design pattern by implementing a payment processing system
    that supports multiple payment providers (Stripe, PayPal, Bank Transfer). Each provider has its own set of
    components: Payment Processor, Receipt Generator, and Refund Handler. The Abstract Factory interface ensures
    that all components from a specific provider are compatible with each other.

    Key Components:
    - Product Interfaces: Define interfaces for Payment Processor, Receipt Generator, and Refund Handler.
    - Concrete Products: Implementations of the product interfaces for each payment provider.
    - Abstract Factory Interface: Declares methods for creating each type of product.
    - Concrete Factories: Implement the abstract factory interface to create families of related products.
    - Client Code: Uses the abstract factory to create and use products without depending on their concrete classes.

    In a sentence: Abstract Factory creates families of related objects that are guaranteed to work together.

    Abstract Factory ensures that when we need multiple related objects 
    (like a payment processor, receipt generator, and refund handler), 
    we get them all from the same 'family' so they're guaranteed to work together.
*/

// ===== Supporting Models =====

public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; }
    public string Message { get; set; }
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
}

public class PaymentDetails
{
    public string CustomerEmail { get; set; }
    public string CustomerName { get; set; }
}

public class Receipt
{
    public string ReceiptId { get; set; }
    public string Format { get; set; } // "PDF", "JSON", "HTML"
    public string Content { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class RefundResult
{
    public bool Success { get; set; }
    public string RefundId { get; set; }
    public string Message { get; set; }
    public decimal RefundedAmount { get; set; }
}

// ===== Product Interfaces =====
public interface IPaymentProcessor
{
    PaymentResult ProcessPayment(decimal amount, PaymentDetails details);
}

public interface IReceiptGenerator
{
    Receipt GenerateReceipt(PaymentResult result);
}

public interface IRefundHandler
{
    RefundResult HandleRefund(string transactionId, decimal amount);
}

// ===== Abstract Factory Interface =====
// This interface ensures that whoever implements it must provide all their components as a cohesive family.
public interface IPaymentServiceFactory
{
    IPaymentProcessor CreatePaymentProcessor();
    IReceiptGenerator CreateReceiptGenerator();
    IRefundHandler CreateRefundHandler();

    string ProviderName {get;}
}

// ===== Concrete Factories =====

// ========== Stripe Family ==========
// ===== Stripe Products =====
public class StripePaymentProcessor : IPaymentProcessor
{
    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        Console.WriteLine($"[Stripe] Processing payment of {amount:C} for {details.CustomerName} ({details.CustomerEmail})");
        var fee = amount * 0.02m + 0.30m;

        return new PaymentResult
        {
            Success = true,
            TransactionId = $"stripe_{Guid.NewGuid():N}",
            Message = "Payment processed successfully via Stripe.",
            Amount = amount,
            Fee = fee
        };
    }
}

public class StripeReceiptGenerator : IReceiptGenerator
{
    public Receipt GenerateReceipt(PaymentResult result)
    {
        Console.WriteLine($"[Stripe] Generating receipt for transaction {result.TransactionId}");
        return new Receipt
        {
            ReceiptId = $"stripe_rcpt_{Guid.NewGuid():N}",
            Format = "PDF",
            Content = $"Stripe Receipt for Transaction {result.TransactionId}, Amount: {result.Amount:C}, Fee: {result.Fee:C}",
            GeneratedAt = DateTime.UtcNow
        };
    }
}

public class StripeRefundHandler : IRefundHandler
{
    public RefundResult HandleRefund(string transactionId, decimal amount)
    {
        Console.WriteLine($"[Stripe] Handling refund of {amount:C} for transaction {transactionId}");
        return new RefundResult
        {
            Success = true,
            RefundId = $"stripe_refund_{Guid.NewGuid():N}",
            Message = "Refund processed successfully via Stripe.",
            RefundedAmount = amount
        };
    }
}

// Stripe Factory

public class StripeServiceFactory : IPaymentServiceFactory
{
    public string ProviderName => "Stripe";

    public IPaymentProcessor CreatePaymentProcessor()
    {
        return new StripePaymentProcessor();
    }

    public IReceiptGenerator CreateReceiptGenerator()
    {
        return new StripeReceiptGenerator();
    }

    public IRefundHandler CreateRefundHandler()
    {
        return new StripeRefundHandler();
    }
}

// ========== PayPal Family ==========
// ===== PayPal Products =====

public class PayPalPaymentProcessor : IPaymentProcessor
{
    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        Console.WriteLine($"[PayPal] Processing ${amount} for {details.CustomerEmail}");
        
        var fee = amount * 0.034m + 0.30m;
        
        return new PaymentResult
        {
            Success = true,
            TransactionId = $"paypal_{Guid.NewGuid():N}",
            Message = "Payment successful via PayPal",
            Amount = amount,
            Fee = fee
        };
    }
}

public class PayPalReceiptGenerator : IReceiptGenerator
{
    public Receipt GenerateReceipt(PaymentResult paymentResult)
    {
        Console.WriteLine($"[PayPal] Generating receipt for {paymentResult.TransactionId}");
        
        // PayPal uses JSON format
        var content = $@"{{
        ""provider"": ""PayPal"",
        ""transactionId"": ""{paymentResult.TransactionId}"",
        ""amount"": {paymentResult.Amount},
        ""fee"": {paymentResult.Fee},
        ""status"": ""{paymentResult.Message}""
        }}";

        return new Receipt
        {
            ReceiptId = $"paypal_rcpt_{Guid.NewGuid():N}",
            Format = "JSON",
            Content = content,
            GeneratedAt = DateTime.UtcNow
        };
    }
}

public class PayPalRefundHandler : IRefundHandler
{
    public RefundResult HandleRefund(string transactionId, decimal amount)
    {
        Console.WriteLine($"[PayPal] Processing refund for {transactionId}: ${amount}");
        
        // PayPal charges a fee even on refunds
        var refundFee = 0.30m;
        
        return new RefundResult
        {
            Success = true,
            RefundId = $"paypal_refund_{Guid.NewGuid():N}",
            Message = $"Refund processed (${refundFee:F2} fee applied)",
            RefundedAmount = amount - refundFee
        };
    }
}

// ===== PayPal Factory =====

public class PayPalServiceFactory : IPaymentServiceFactory
{
    public string ProviderName => "PayPal";

    public IPaymentProcessor CreatePaymentProcessor()
    {
        return new PayPalPaymentProcessor();
    }

    public IReceiptGenerator CreateReceiptGenerator()
    {
        return new PayPalReceiptGenerator();
    }

    public IRefundHandler CreateRefundHandler()
    {
        return new PayPalRefundHandler();
    }
}

//===== Bank Transfer Family =========
public class BankTransferPaymentProcessor : IPaymentProcessor
{
    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        Console.WriteLine($"[BankTransfer] Processing bank transfer of {amount:C} for {details.CustomerEmail}") ;
        // Simulate bank transfer processing logic
        return new PaymentResult
        {
            Success = true,
            TransactionId = $"bank_{Guid.NewGuid():N}",
            Message = "Bank transfer initiated (2-3 business days)",
            Amount = amount,
            Fee = 5.00m // Flat fee for bank transfers
        };
    }
}

public class BankTransferReceiptGenerator : IReceiptGenerator
{
    public Receipt GenerateReceipt(PaymentResult result)
{
    Console.WriteLine($"[BankTransfer] Generating receipt for transaction {result.TransactionId}");
    
    var content = $@"
    =================================
    BANK TRANSFER RECEIPT (PDF)
    =================================
    Transaction ID: {result.TransactionId}
    Amount: {result.Amount:C}
    Processing Fee: {result.Fee:C}
    Total Charged: {(result.Amount + result.Fee):C}
    Status: {result.Message}
    Expected Completion: 2-3 business days
    =================================";

    return new Receipt
    {
        ReceiptId = $"bank_rcpt_{Guid.NewGuid():N}",
        Format = "PDF",
        Content = content,
        GeneratedAt = DateTime.UtcNow
    };
}
}

public class BankTransferRefundHandler : IRefundHandler
{
    public RefundResult HandleRefund(string transactionId, decimal amount)
    {
        Console.WriteLine($"[BankTransfer] Handling refund of {amount:C} for transaction {transactionId}");
        return new RefundResult
        {
            Success = true,
            RefundId = $"bank_refund_{Guid.NewGuid():N}",
            Message = "Refund will be processed in 5-7 business days",
            RefundedAmount = amount
        };
    }
}

public class BankTransferServiceFactory : IPaymentServiceFactory
{
    public string ProviderName => "BankTransfer";

    public IPaymentProcessor CreatePaymentProcessor()
    {
        return new BankTransferPaymentProcessor();
    }

    public IReceiptGenerator CreateReceiptGenerator()
    {
        return new BankTransferReceiptGenerator();
    }

    public IRefundHandler CreateRefundHandler()
    {
        return new BankTransferRefundHandler();
    }
}

// == Client Code (using factory) ==

public class PaymentService
{
    private readonly IPaymentServiceFactory _factory;

    public PaymentService(IPaymentServiceFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public void ProcessTransaction(decimal amount, PaymentDetails details)
    {
        Console.WriteLine($"\n========== {_factory.ProviderName} Transaction ==========");
        Console.WriteLine($"[Using {_factory.ProviderName} family - all components guaranteed compatible]\n");

        // Create entire family of objects from one factory
        var processor = _factory.CreatePaymentProcessor();
        var receiptGenerator = _factory.CreateReceiptGenerator();
        var refundHandler = _factory.CreateRefundHandler();

        // 1. Process payment
        var paymentResult = processor.ProcessPayment(amount, details);
        
        if (paymentResult.Success)
        {
            Console.WriteLine($"  {paymentResult.Message}");
            Console.WriteLine($"  Transaction ID: {paymentResult.TransactionId}");
            Console.WriteLine($"  Amount: {paymentResult.Amount:C}");
            Console.WriteLine($"  Fee: {paymentResult.Fee:C}\n");

            // 2. Generate receipt
            var receipt = receiptGenerator.GenerateReceipt(paymentResult);
            Console.WriteLine($"  Receipt generated ({receipt.Format} format)");
            Console.WriteLine($"  Receipt ID: {receipt.ReceiptId}");
            Console.WriteLine($"  Generated: {receipt.GeneratedAt:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine(receipt.Content);

            // 3. Simulate refund scenario
            Console.WriteLine("\n--- Simulating Refund ---");
            var refundResult = refundHandler.HandleRefund(paymentResult.TransactionId, amount);

            if (refundResult.Success)
            {
                Console.WriteLine($"  {refundResult.Message}");
                Console.WriteLine($"  Refund ID: {refundResult.RefundId}");
                Console.WriteLine($"  Refunded Amount: {refundResult.RefundedAmount:C}");
            }
            else
            {
                Console.WriteLine($"  {refundResult.Message}");
            }
        }
        else
        {
            Console.WriteLine($"  {paymentResult.Message}");
        }

        Console.WriteLine($"\n========== End {_factory.ProviderName} Transaction ==========\n");
    }
}

// ===== Factory Selector (Similar to Factory Method) =====

public class PaymentServiceFactoryProvider
{
    private readonly Dictionary<string, IPaymentServiceFactory> _factories;

    public PaymentServiceFactoryProvider()
    {
        _factories = new Dictionary<string, IPaymentServiceFactory>(StringComparer.OrdinalIgnoreCase)
        {
            { "Stripe", new StripeServiceFactory() },
            { "PayPal", new PayPalServiceFactory() },
            { "BankTransfer", new BankTransferServiceFactory() }
        };
    }

    public IPaymentServiceFactory GetFactory(string providerName)
    {
        if(_factories.TryGetValue(providerName, out var factory))
        {
            return factory;
        }
        throw new ArgumentException($"No factory found for provider: {providerName}");
    }

    public IEnumerable<string> GetAvailableProviders()
    {
        return _factories.Keys;
    }
}

// ===== Usage Example =====
public class Program
{
    public static void Main()
    {
        var factoryProvider = new PaymentServiceFactoryProvider();
        var details = new PaymentDetails
        {
            CustomerName = "John Doe",
            CustomerEmail = "john.doe@example.com"
        };

        var stripeFactory = factoryProvider.GetFactory("Stripe");
        var stripeService = new PaymentService(stripeFactory);
        stripeService.ProcessTransaction(100.00m, details);

        var paypalFactory = factoryProvider.GetFactory("PayPal");
        var paypalService = new PaymentService(paypalFactory);
        paypalService.ProcessTransaction(150.00m, details);

        var bankFactory = factoryProvider.GetFactory("BankTransfer");
        var bankService = new PaymentService(bankFactory);
        bankService.ProcessTransaction(250.00m, details);
    }
}