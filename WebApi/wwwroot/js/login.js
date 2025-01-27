const loginForm = document.getElementById('loginForm');

loginForm.addEventListener('submit', async (event) => {
    event.preventDefault();

    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    const loginDto = {
        Email: username,
        Lozinka: password
    };


    try {
        const response = await fetch('http://localhost:5289/api/Korisnik/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(loginDto),
        });

        if (response.ok) {
            const result = await response.json();
            alert('Login successful!');
            localStorage.setItem('token', result.token);
            window.location.href = '/html/logs.html';
        } else {
            alert('Login failed: ' + (await response.text()));
            
        }
    } catch (error) {
        console.error('Error:', error);
        alert('An error occurred. Please try again.');
    }
});
