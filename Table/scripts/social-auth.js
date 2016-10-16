function RegisterUsingFacebook() {
    FB.login(function (response) {
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
    });
}

function RegisterUsingVK() {
    VK.Auth.login(function (response) {
        if (response.session) {
            console.log(response.session.user.first_name + " " + response.session.user.last_name);
            console.log(response.session.user.id);
            console.log(response.session.user.domain);
        }
        $.ajax({
            type: "POST",
            async: false,
            url: '/Auth/RegisterVK',
            contentType: 'application/x-www-form-urlencoded; charset=windows-1251',
            data: {
                userid: response.session.user.id,
                name: response.session.user.first_name + " " + response.session.user.last_name,
                email: response.session.user.domain
            },
            success: window.location.reload()
        });
    });
}

function RegisterUsingGoogle(googleUser) {
        var profile = googleUser.getBasicProfile();
        console.log('ID: ' + profile.getId()); // Do not send to your backend! Use an ID token instead.
        console.log('Name: ' + profile.getName());
        console.log('Image URL: ' + profile.getImageUrl());
        console.log('Email: ' + profile.getEmail());
}

function LogOutFromSocialNetworks() {
    FB.logout(function (response) {
        // user is now logged out
    });
    VK.Auth.logout(function (response) {
        //user is now logged out
    });
}