import { useEffect, useState } from "react";
import api from "../api/httpClient";
import { useNavigate } from "react-router-dom";
import "./Profile.css";

export default function Profile() {
    const navigate = useNavigate();

    const [profile, setProfile] = useState(null);
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        const loadProfile = async () => {
            try {
               
                const profileRes = await api.get("/user/userprofiles");
                setProfile(profileRes.data);

                const userRes = await api.get(`/auth/users/${profileRes.data.id}`);
                setFirstName(userRes.data.firstName || "");
                setLastName(userRes.data.lastName || "");
            } catch (error) {
                alert("Failed to load profile");
            }
        };

        loadProfile();
    }, []);

    const saveProfile = async () => {
        try {
            setSaving(true);

           
            await api.put(`/auth/users/${profile.id}`, {
                firstName,
                lastName,
            });

            if (currentPassword && newPassword) {
                await api.put(`/auth/users/${profile.id}/password`, {
                    currentPassword,
                    newPassword,
                });
            }

            alert("Profile updated successfully");

            setCurrentPassword("");
            setNewPassword("");
        } catch (error) {
            alert("Failed to update profile or password");
        } finally {
            setSaving(false);
        }
    };

    if (!profile) {
        return <div className="profile-loading">Loading...</div>;
    }

    return (
        <div className="profile-container">
            <div className="profile-wrapper">
                <div className="profile-header">
                    <h1>My Profile</h1>
                    <button
                        className="back-btn"
                        onClick={() => navigate("/dashboard")}
                    >
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

                    <div className="profile-row">
                        <label>First Name</label>
                        <input
                            value={firstName}
                            onChange={(e) => setFirstName(e.target.value)}
                        />
                    </div>

                    <div className="profile-row">
                        <label>Last Name</label>
                        <input
                            value={lastName}
                            onChange={(e) => setLastName(e.target.value)}
                        />
                    </div>

                    <div className="profile-row">
                        <label>Current Password</label>
                        <input
                            type="password"
                            placeholder="Leave blank to keep current password"
                            value={currentPassword}
                            onChange={(e) => setCurrentPassword(e.target.value)}
                        />
                    </div>

                    <div className="profile-row">
                        <label>New Password</label>
                        <input
                            type="password"
                            placeholder="Leave blank to keep current password"
                            value={newPassword}
                            onChange={(e) => setNewPassword(e.target.value)}
                        />
                    </div>

                    <button
                        className="save-btn"
                        disabled={saving}
                        onClick={saveProfile}
                    >
                        {saving ? "Saving..." : "Save Profile"}
                    </button>
                </div>
            </div>
        </div>
    );
}
