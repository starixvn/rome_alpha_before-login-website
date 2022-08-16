var clock;

$(document).ready(function () {
    var countdownTime = $('#countdown-time').val();

    if (countdownTime) {
        var clock;

        clock = $('.clock').FlipClock({
            clockFace: 'HourCounter',
            autoStart: false,
            callbacks: {
                stop: function () {
                    window.location.href = window.location.href;
                }
            }
        });

        clock.setTime(parseInt(countdownTime));
        clock.setCountdown(true);
        clock.start();
    }
});