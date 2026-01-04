import { useState } from "react";
import api from "../api/httpClient";
import { useAuth } from "../auth/AuthContext";
import { useNavigate, Link } from "react-router-dom";
import "./Login.css";


function Login(){
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const { setUser } = useAuth();
    const navigate = useNavigate();

    const handleLogin = async () => {
        try{
            const response = await api.post("/auth/login", {
                email,
                password,
            });

            const token = response.data.token;

            sessionStorage.setItem("token", token);

            setUser({ email });

            navigate("/dashboard");
        } catch (err) {
            alert("Invalid credentials")
        }
    };

    return (
    <div className="login-page">
        <div className="login-card">
            <h1>Welcome Back</h1>
            <p className="subtitle">Login to your account</p>

        <label>Email</label>
        <input
            type="email"
            placeholder="Enter your email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
        />

        <label>Password</label>
        <input
            type="password"
            placeholder="Enter your password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
        />

        <button onClick={handleLogin}>Login</button>

        <p className="signup-text">
            Don’t have an account? <Link to="/register">Sign Up</Link>
        </p>
      </div>
    </div>
  );
}


export default Login;