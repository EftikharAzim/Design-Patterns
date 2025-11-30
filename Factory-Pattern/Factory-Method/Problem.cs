/*
    The Problem Simple Factory Can't Solve:
    Imagine our platform operates in multiple regions, each with different payment providers:
    
        US Region: Stripe, PayPal, Venmo
        EU Region: Stripe, PayPal, SEPA Transfer
        Asia Region: Alipay, WeChat Pay, Bank Transfer

        We could build this with Simple Factory using if-else chains:
*/

public class PaymentProcessorFactory
{
    public IPaymentProcessor GetPaymentProcessor(string region, string paymentType)
    {
        if(region == "US")
        {
            return paymentType.ToLower() switch
            {
                "stripe" => new StripeProcessor(),
                "paypal" => new PayPalProcessor(),
                "venmo" => new VenmoProcessor(),
                _ => throw new NotSupportedException($"Payment type {paymentType} is not supported in US region.")
            };
        }
        else if(region == "EU")
        {
            // Different set of payment processors
        }
        // Additional regions...
    }
}
