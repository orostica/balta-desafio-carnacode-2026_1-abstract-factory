// DESAFIO: Sistema de Pagamentos Multi-Gateway
// PROBLEMA: Uma plataforma de e-commerce precisa integrar com múltiplos gateways de pagamento
// (PagSeguro, MercadoPago, Stripe) e cada gateway tem componentes específicos (Processador, Validador, Logger)
// O código atual está muito acoplado e dificulta a adição de novos gateways

using System;

namespace DesignPatternChallenge
{
    // Contexto: Sistema de pagamentos que precisa trabalhar com diferentes gateways
    // Cada gateway tem sua própria forma de processar, validar e logar transações
    
    public interface IValidator{}
    public interface IProcessor{}
    public interface ILogger{}

    public interface IPaymentGateway
    {
        IValidator GetValidator();
        IProcessor GetProcessor();
        ILogger GetLogger();
    }

    public class PagSeguroGateway : IPaymentGateway
    {
        public IValidator GetValidator() => new PagSeguroValidator();
        public IProcessor GetProcessor() => new PagSeguroProcessor();
        public ILogger GetLogger() => new PagSeguroLogger();
    }
    public class MercadoPagoGateway : IPaymentGateway
    {
        public IValidator GetValidator() => new MercadoPagoValidator();
        public IProcessor GetProcessor() => new MercadoPagoProcessor();
        public ILogger GetLogger() => new MercadoPagoLogger();
    }

    public class StripeGateway : IPaymentGateway
    {
        public IValidator GetValidator() => new StripeValidator();
        public IProcessor GetProcessor() => new StripeProcessor();
        public ILogger GetLogger() => new StripeLogger();
    }

    public class PaymentService
    {
        private readonly string _gateway;
        private readonly IPaymentGateway _paymentGateway;

        public PaymentService(string gateway, IPaymentGateway paymentGateway)
        {
            _gateway = gateway;
            _paymentGateway = paymentGateway;
        }

        public void ProcessPayment(decimal amount, string cardNumber)
        {
            var validator = _paymentGateway.GetValidator();
            var processor = _paymentGateway.GetProcessor();
            var logger = _paymentGateway.GetLogger();

            if (!((dynamic)validator).ValidateCard(cardNumber))
            {
                Console.WriteLine($"{_gateway}: Cartão inválido");
                return;
            }

            var result = ((dynamic)processor).ProcessTransaction(amount, cardNumber);

            ((dynamic)logger).Log($"Transação processada: {result}");
        }
    }

    // Componentes do PagSeguro
    public class PagSeguroValidator
    {
        public bool ValidateCard(string cardNumber) 
        {
            Console.WriteLine("PagSeguro: Validando cartão...");
            return cardNumber.Length == 16;
        }
    }

    public class PagSeguroProcessor
    {
        public string ProcessTransaction(decimal amount, string cardNumber)
        {
            Console.WriteLine($"PagSeguro: Processando R$ {amount}...");
            return $"PAGSEG-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }

    public class PagSeguroLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[PagSeguro Log] {DateTime.Now}: {message}");
        }
    }

    // Componentes do MercadoPago
    public class MercadoPagoValidator
    {
        public bool ValidateCard(string cardNumber)
        {
            Console.WriteLine("MercadoPago: Validando cartão...");
            return cardNumber.Length == 16 && cardNumber.StartsWith("5");
        }
    }

    public class MercadoPagoProcessor
    {
        public string ProcessTransaction(decimal amount, string cardNumber)
        {
            Console.WriteLine($"MercadoPago: Processando R$ {amount}...");
            return $"MP-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }

    public class MercadoPagoLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[MercadoPago Log] {DateTime.Now}: {message}");
        }
    }

    // Componentes do Stripe
    public class StripeValidator
    {
        public bool ValidateCard(string cardNumber)
        {
            Console.WriteLine("Stripe: Validando cartão...");
            return cardNumber.Length == 16 && cardNumber.StartsWith("4");
        }
    }

    public class StripeProcessor
    {
        public string ProcessTransaction(decimal amount, string cardNumber)
        {
            Console.WriteLine($"Stripe: Processando ${amount}...");
            return $"STRIPE-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }

    public class StripeLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[Stripe Log] {DateTime.Now}: {message}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sistema de Pagamentos ===\n");

            // Problema: Cliente precisa saber qual gateway está usando
            // e o código de processamento está todo acoplado
            var pagSeguroService = new PaymentService("pagseguro");
            pagSeguroService.ProcessPayment(150.00m, "1234567890123456");

            Console.WriteLine();

            var mercadoPagoService = new PaymentService("mercadopago");
            mercadoPagoService.ProcessPayment(200.00m, "5234567890123456");

            Console.WriteLine();

            // Pergunta para reflexão:
            // - Como adicionar um novo gateway sem modificar PaymentService?
            // - Como garantir que todos os componentes de um gateway sejam compatíveis entre si?
            // - Como evitar criar componentes de gateways diferentes acidentalmente?
        }
    }
}
