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

    $('#stud-sign-in').click(function () {
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
                    window.location.href = '/Student/Index'
                } else {
                    $('#loginValidation1').show();
                    $('#loginFailed1').text(response);
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