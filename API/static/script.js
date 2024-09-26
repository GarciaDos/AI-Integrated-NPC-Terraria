function sendQuestion() {
    const questionInput = document.getElementById('questionInput').value;
  
    const url = "http://localhost:8000/generate_text";
    const payload = {
        question: questionInput
    };
  
    fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(payload)
    })
    .then(response => response.json())
    .then(responseData => {
        const answer = responseData.answer;
        const npcBox = document.getElementById('npcBox');
        
        // Clear the previous answer before typing the new one
        npcBox.innerHTML = '';
        
        // Initialize the Typed.js animation
        new Typed('#npcBox', {
          strings: [answer],    // Use the response as the text to type
          typeSpeed: 50,        // Speed of typing
          backSpeed: 30,        // Speed of backspacing
          startDelay: 100,      // Delay before typing starts
          loop: false           // No looping
        });
    })
    .catch(error => {
        console.error("Error:", error);
    });
  }
  