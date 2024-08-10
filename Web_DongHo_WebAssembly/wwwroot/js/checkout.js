
class OrderModel {
    constructor(fullName = '', phone = '', address = '', paymentMethod = 'Cash') {
        this.fullName = fullName;
        this.phone = phone;
        this.address = address;
        this.paymentMethod = paymentMethod;
    }
}

window.initializeCheckout = (totalAmount, username, orderModelData) => {

    // Create an OrderModel instance from the received data
    const orderModel = new OrderModel(
        orderModelData.fullName,
        orderModelData.phone,
        orderModelData.address,
        orderModelData.paymentMethod
    );


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

                var completePurchaseRequest = {
                    FullName: orderModel.fullName,
                    Phone: orderModel.phone,
                    Address: orderModel.address,
                    PaymentMethod: 'PayPal'
                };

                fetch(`https://localhost:44355/api/checkout/completePurchase?username=${username}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(completePurchaseRequest)
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            Swal.fire({
                                title: 'Thành công!',
                                text: 'Thanh toán đã được xử lý thành công.',
                                icon: 'success',
                                confirmButtonText: 'OK'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    window.location.href = '/historyBill'; 
                                }
                            });
                        } else {
                            console.error('Purchase completion failed:', data.message);
                            Swal.fire({
                                title: 'Lỗi!',
                                text: 'Có lỗi khi thực hiện thanh toán',
                                icon: 'error',
                                confirmButtonText: 'OK'
                            });
                        }
                    })
                    .catch(error => {
                        
                    });
            });
        }
        , onCancel: function (data) {
            Swal.fire({
                title: 'Đã hủy!',
                text: 'Bạn đã hủy quá trình thanh toán.',
                icon: 'warning',
                confirmButtonText: 'OK'
            });
        },
        onError: function (err) {
            console.error('Error completing purchase:', err);

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
function showAlertAndConfirm(message) {
    alert(message);
    return true; // You can change the condition or add more logic here if needed
}