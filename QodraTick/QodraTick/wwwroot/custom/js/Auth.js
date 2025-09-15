// ✅ استفاده از JavaScript برای فراخوانی API
var loginData = System.Text.Json.JsonSerializer.Serialize(new
    {
        username = Username,
        password = Password,
        rememberMe = RememberMe
    });

var response = await JSRuntime.InvokeAsync < string > ("eval", $@"
fetch('/api/auth/login', {{
    method: 'POST',
    headers: {{
    'Content-Type': 'application/json'
}},
    body: '{loginData}'
                }})
    .then(response => response.json())
    .then(data => data.success ? 'success' : data.message)
    .catch(error => 'خطا: ' + error.message)
");

if (response == "success") {
    successMessage = "ورود موفقیت‌آمیز! در حال انتقال...";
    StateHasChanged();

    await Task.Delay(1500);
    Navigation.NavigateTo("/", true);
}
else {
    errorMessage = response;
}