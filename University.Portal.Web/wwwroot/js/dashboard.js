$(function () {

    const studentsignInButton = document.getElementById('studentsignIn');
    const universitysignInButton = document.getElementById('universitysignIn');
    const container = document.getElementById('container');

    $('#loginValidation1').hide();

    studentsignInButton.addEventListener('click', () => {
        container.classList.remove('right-panel-active');        
        $('#loginValidation1').hide();
    });

    universitysignInButton.addEventListener('click', () => {
        container.classList.add('right-panel-active');
        $('#loginValidation').hide();
    });

    $('#univ-sign-in').click(function () {

        $('#univ-sign-in')[0].innerHTML = '<i class="fa fa-refresh fa-spin"></i> Signing In';
        $('#univ-sign-in').addClass('disabled');

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
                    window.location.href = '/University';
                } else {
                    $('#loginValidation').show();
                    $('#loginFailed').text(response);
                    $('#univ-sign-in')[0].innerHTML = 'Sign In';
                    $('#univ-sign-in').removeClass('disabled');
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

    $('#stud-sign-in').click(function () {

        $('#stud-sign-in')[0].innerHTML = '<i class="fa fa-refresh fa-spin"></i> Signing In';
        $('#stud-sign-in').addClass('disabled');

        $('#loginValidation1').hide();
        var myData = $('#stud-sign-in-form').serialize();
        $.ajax({
            type: "POST",
            url: "/Dashboard/StudentLogin",
            data: myData,
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            dataType: "html",
            success: function (response) {
                if (response == '"Success"') {
                    //window.location.href = '/Student/FeePayment';
                    window.location.href = '/Notification/Notification';                    
                } else {
                    $('#loginValidation1').show();
                    $('#loginFailed1').text(response);
                    $('#stud-sign-in')[0].innerHTML = 'Sign In';
                    $('#stud-sign-in').removeClass('disabled');
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