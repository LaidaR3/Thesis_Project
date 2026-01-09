import { useEffect, useState } from "react";
import api from "../api/httpClient";
import { useNavigate } from "react-router-dom";
import "./Profile.css";

export default function Profile() {
    const [profile, setProfile] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        api
            .get("/user/userprofiles")
            .then((res) => setProfile(res.data))
            .catch(() => alert("Failed to load profile"));
    }, []);

    if (!profile) return <div className="profile-loading">Loading...</div>;

    return (
        <div className="profile-container">
            <div className="profile-wrapper">
                <div className="profile-header">
                    <h1>My Profile</h1>
                    <button className="back-btn" onClick={() => navigate("/dashboard")}>
                        Back to Dashboard
                    </button>
                </div>


                <div className="profile-card">
                    <div className="profile-row">
                        <label>User ID</label>
                        <input value={profile.id} disabled />
                    </div>

                    <div className="profile-row">
                        <label>Email</label>
                        <input value={profile.email} disabled />
                    </div>

                    <div className="profile-row">
                        <label>Role</label>
                        <input value={profile.role} disabled />
                    </div>
                </div>

            </div>
        </div>
    );
}
