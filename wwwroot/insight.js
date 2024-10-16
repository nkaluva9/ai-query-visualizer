const queryInput = document.getElementById('queryInput');
const micButton = document.getElementById('micButton');
const micImage = document.getElementById('micImage');
// Replace these with your Azure Speech Service API key and region
const azureSpeechKey = '24f9eece04fc43ccb47fc93dbebfcbd2';
const azureRegion = 'eastus2';

function renderChart(chartId, chartData, chartOptions) {
    var ctx = document.getElementById(chartId).getContext('2d');
    new Chart(ctx, {
        type: chartData.type,
        data: chartData.data,
        options: {
            responsive: false,
            maintainAspectRatio: false,
            ...chartOptions
        }
    });
}

function renderChart2(chartId, chartData, chartOptions) {
    var ctx = document.getElementById(chartId);
    if (ctx) {
        ctx = ctx.getContext('2d');
        new Chart(ctx, {
            type: chartData.type,
            data: chartData.data,
            options: chartOptions
        });
    } else {
        console.error("Canvas element not found:", chartId);
    }
}
function startAzureSpeechToText() {
    if (!azureSpeechKey || !azureRegion) {
        alert('Please set your Azure Speech Key and Region.');
        return;
    }

    // Set up speech configuration
    const speechConfig = SpeechSDK.SpeechConfig.fromSubscription(azureSpeechKey, azureRegion);
    speechConfig.speechRecognitionLanguage = 'en-US'; // Set language

    const audioConfig = SpeechSDK.AudioConfig.fromDefaultMicrophoneInput(); // Use default microphone
    const recognizer = new SpeechSDK.SpeechRecognizer(speechConfig, audioConfig);

    micButton.disabled = true;
    queryInput.value = "Listening...";
    queryInput.classList.add('loading-text');
    micImage.src = "https://img.icons8.com/?size=100&id=12798&format=png&color=ffffff"; // Listening visual

    recognizer.recognizeOnceAsync(
        function (result) {
            if (result.reason === SpeechSDK.ResultReason.RecognizedSpeech) {
                queryInput.value = result.text;
                debugger;
                queryInput.classList.remove('loading-text');
                //queryInput.dispatchEvent(new Event('input'));
                DotNet.invokeMethodAsync('AIQueryVisualizer', 'UpdateUserInput', result.text)
                    .then(() => {
                        console.log('UserInput updated successfully in Blazor.');
                    })
                    .catch(err => {
                        console.error('Error updating UserInput in Blazor:', err);
                    });
            } else {
                console.error('Speech recognition failed: ' + result.errorDetails);
                alert('Speech recognition failed: ' + result.errorDetails);
            }
            resetMicButton();
            recognizer.close();
        },
        function (err) {
            console.error('Error: ' + err);
            alert('Error occurred: ' + err);
            resetMicButton();
            recognizer.close();
        });
}
function resetMicButton() {
    micButton.disabled = false;
    queryInput.placeholder = "Enter a query to send to the bot";
    micImage.src = "https://img.icons8.com/ios-filled/50/ffffff/microphone.png"; // Reset to microphone image
}
//window.onload = function () {
//    document.getElementById('micButton').addEventListener('click', function () {
//        startAzureSpeechToText();
//    });
//};