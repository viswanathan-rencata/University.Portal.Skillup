$(function () {

    const studentsignInButton = document.getElementById('studentsignIn');
    const universitysignInButton = document.getElementById('universitysignIn');
    const container = document.getElementById('container');

    studentsignInButton.addEventListener('click', () => {
        container.classList.remove('right-panel-active');        
    });

    universitysignInButton.addEventListener('click', () => {
        container.classList.add('right-panel-active');
    });
});