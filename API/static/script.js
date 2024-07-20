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
      // Find the div with id npcBox and set its innerHTML to the answer
      const npcBox = document.getElementById('npcBox');
      npcBox.innerHTML = answer;
  })
  .catch(error => {
      console.error("Error:", error);
  });
}