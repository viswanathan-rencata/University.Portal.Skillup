$(function () {

    const studentsignInButton = document.getElementById('studentsignIn');
    const universitysignInButton = document.getElementById('universitysignIn');
    const container = document.getElementById('container');

    studentsignInButton.addEventListener('click', () => {
        container.classList.remove('right-panel-active');        
    });

    universitysignInButton.addEventListener('click', () => {
        container.classList.add('right-panel-active');
        $('#loginValidation').hide();
    });

    $('#univ-sign-in').click(function () {
        $('#loginValidation').hide();
        var myData = $('#univ-sign-in-form').serialize();
        $.ajax({
            type: "POST",
            url: "/Dashboard/Login",
            data: myData,
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            dataType: "html",
            success: function (response) {
                if (response == '"Success"') {
                    window.location.href = '/University/Index'
                } else {
                    $('#loginValidation').show();
                    $('#loginFailed').text(response);
                }
                
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });    
    });
});