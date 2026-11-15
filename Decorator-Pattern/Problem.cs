/*
    Our payment processor works great, but now we need to add optional features:
        1. Logging - record every payment attempt
        2. Fraud detection - check transactions for suspicious activity
        3. Retry logic - automatically retry failed payments
        4. Currency conversion - convert amounts to different currencies
        5. Notification - send notifications for payments
    
    The catch: Not all payments need all features. We want to mix and match:
        1. Basic Payment (no extras)
        2. Payment with logging
        3. Payment with logging + fraud detection
        4. Payment with fraud detection + retry + notification
        5. Any combination with want...

    Problems:
        1. Violates Single Responsibility Principle (one class doing 6 things)
        2. Hard to test individual features
        3. Adding new features means modifying this class (violates OCP)
        4. Can't compose features in different orders
*/

// Approach 1: Inheritence explosion
public class PaymentProcessor { }
public class LoggingPaymentProcessor : PaymentProcessor { }
public class FraudDetectionPaymentProcessor : PaymentProcessor { }
public class LoggingWithFraudDetectionPaymentProcessor : PaymentProcessor { }
public class RetryPaymentProcessor : PaymentProcessor { }
/*
    2^5 = 32 classes for 5 features!
    Combinational explosion. With 5 features, we'd need 32 classes to cover all combinations.
*/

// Approach 2: Boolean flags
public class PaymentProcessor
{
    private bool _enableLogging;
    private bool _enableFraudDetection;
    private bool _enableRetry;
    private bool _enableCurrencyConversion;
    private bool _enableNotification;

    public PaymentResult ProcessPayment(decimal amount, PaymentDetails details)
    {
        if (_enableLogging)
        {
            // logging logic
        }

        if (_enableFraudDetection)
        {
            // fraud detection logic
        }

        // actual payment processing

        if (_enableRetry)
        {
            // retry logic
        }

        if (_enableCurrencyConversion)
        {
            // currency conversion logic
        }

        if (_enableNotification)
        {
            // notification logic
        }
    }
}