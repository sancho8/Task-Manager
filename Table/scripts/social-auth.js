function RegisterUsingFacebook() {
    FB.api('/me', { locale: 'en_US', fields: 'name, email, id' },
        function (response) {
            console.log(response.name);
            console.log(response.id);
            console.log(response.email);
            $.ajax({
                type: "POST",
                async: false,
                url: '/Auth/RegisterFacebook',
                data: {
                    userid: response.id,
                    name: response.name,
                    email: response.email
                },
                success: window.location.reload()
            });
        });
}

function RegisterUsingVK() {
    VK.Auth.login(function (response) {
        if (response.session) {
           alert(response.session.user.first_name);
        } 
    });
}

function LogOutFromSocialNetworks() {
    FB.logout(function (response) {
        // user is now logged out
    });
}