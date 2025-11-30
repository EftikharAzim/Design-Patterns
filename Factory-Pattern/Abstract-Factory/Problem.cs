/*

    Abstract Factory Pattern: The Problem
    Scenario: Our payment system now needs more than just payment processing. For each payment provider, we also need:

    Payment Processor — processes the payment
    Receipt Generator — generates receipts in the provider's format
    Refund Handler — handles refunds through the provider's API

    The catch: These components must be compatible. We can't mix:

    Stripe payment + PayPal receipt + Venmo refund (inconsistent)
    Stripe payment + Stripe receipt + Stripe refund (consistent family)

    This is where Abstract Factory shines: it creates families of related objects that are guaranteed to work together.
*/

// The Naive Approach (Why It Fails)
// Try using multiple Factory Methods:

public abstract class PaymentFactory
{
    public abstract IPaymentProcessor CreatePaymentProcessor();
    public abstract IReceiptGenerator CreateReceiptGenerator();
    public abstract IRefundHandler CreateRefundHandler();
}

// This works but we don't have a guarantee that someone won't mix and match incompatible components.

// var processor = stripeFactory.CreatePaymentProcessor();
// var receipt = paypalFactory.CreateReceiptGenerator();
// var refund = venmoFactory.CreateRefundHandler();
// This leads to runtime errors and inconsistent behavior.