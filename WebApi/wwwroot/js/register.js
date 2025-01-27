const registerForm = document.getElementById('registerForm');

registerForm.addEventListener('submit', async (event) => {
    event.preventDefault();

    const firstName = document.getElementById('firstName').value;
    const lastName = document.getElementById('lastName').value;
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirmPassword').value;

    if (password !== confirmPassword) {
        alert('Passwords do not match. Please try again.');
        return;
    }

    const responseSalt = await fetch('http://localhost:5289/api/Korisnik/getSalt', {
        method: 'GET'
    });

    if (!responseSalt.ok) {
        alert('An error occurred while generating the salt.');
        return;
    }

    const salt = await responseSalt.text();

    const korisnikDto = {
        Ime: firstName,
        Prezime: lastName,
        Email: email,
        Lozinka: password,
        Salt: salt,
        Narudzbas: []
    };

    try {
        const response = await fetch('http://localhost:5289/api/Korisnik/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(korisnikDto),
        });

        if (response.ok) {
            alert('Registration successful! You can now log in.');
            window.location.href = '/html/login.html';
        } else {
            alert('Registration failed: ' + (await response.text()));
        }
    } catch (error) {
        console.error('Error:', error);
        alert('An error occurred. Please try again.');
    }
});
