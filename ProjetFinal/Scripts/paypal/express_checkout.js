$(document).ready(function () {
    // Render the PayPal button
    paypal.Button.render({

        // Set your environment
        env: 'sandbox', // sandbox | production

        // PayPal Client IDs
        client: {
            sandbox: 'AZDxjDScFpQtjWTOUtWKbyN_bDt4OgqaF4eYXlewfBP4-8aqX3PiV8e1GWU6liB2CUXlkA59kJXE7M6R',
        },

        // Wait for the PayPal button to be clicked
        payment: function() {
            debugger;
            // Make a client-side call to the REST api to create the payment
            return paypal.rest.payment.create(this.props.env, this.props.client, {
                transactions: [
                    {
                        amount: { total: $('#_price').val().replace(',', '.'), currency: 'CAD' }
                    }
                ]
            });
        },

        // Wait for the payment to be authorized by the customer

        onAuthorize: function(data, actions) {

            // Execute the payment

            return actions.payment.execute().then(function() {
                document.querySelector('#paypal-button-container').innerText = 'Paiement complété!';
                window.location = $('#confirmPurchaseLink').val();
            });
        }

    }, '#paypal-button-container');
});