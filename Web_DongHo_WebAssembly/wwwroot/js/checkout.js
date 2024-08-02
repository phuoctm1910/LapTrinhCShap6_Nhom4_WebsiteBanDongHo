window.logPaymentMethod = (paymentMethod) => {
    console.log('Selected payment method:', paymentMethod);
};

window.initializeCheckout = (totalAmount) => {
    console.log('initializeCheckout called with totalAmount:', totalAmount);
    paypal.Buttons({
        style: {
            layout: 'vertical',
            color: 'blue',
            shape: 'rect',
            label: 'paypal'
        },
        createOrder: function (data, actions) {
            var exchangeRate = 23000; // Exchange rate from VND to USD
            var totalAmountUSD = (totalAmount / exchangeRate).toFixed(2);
            console.log('Creating order with amount in USD:', totalAmountUSD);

            return actions.order.create({
                purchase_units: [{
                    amount: {
                        currency_code: 'USD',
                        value: totalAmountUSD
                    }
                }]
            });
        },
        onApprove: function (data, actions) {
            return actions.order.capture().then(function (details) {
                console.log('Order approved:', details);
                DotNet.invokeMethodAsync('Web_DongHo_WebAssembly', 'OnPaypalPaymentSuccess', details);
            });
        }
    }).render('#paypal-button-container');
};

window.showPayPalButton = () => {
    console.log('Showing PayPal button');
    document.querySelector('#paypal-container').style.display = 'block';
    document.querySelector('#complete').style.display = 'none';
};

window.hidePayPalButton = () => {
    console.log('Hiding PayPal button');
    document.querySelector('#paypal-container').style.display = 'none';
    document.querySelector('#complete').style.display = 'block';
};

window.processPaypalPayment = (orderModel) => {
    console.log('Processing PayPal payment with orderModel:', orderModel);
    // Here you can send the orderModel to the server or process it further in JavaScript
};
