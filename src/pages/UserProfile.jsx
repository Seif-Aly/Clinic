import React, { useState, useEffect } from "react";
import axios from "axios";

export default function UserProfile() {
    const [profile, setProfile] = useState(null);
    const [password, setPassword] = useState(""); 
    const [isEditing, setIsEditing] = useState(false);

    
    useEffect(() => {
        axios.get("http://localhost:5048/api/UserProfile/1") 
            .then((response) => setProfile(response.data))
            .catch((error) => console.error("Error fetching profile:", error));
    }, []);

    const handleSave = () => {
        
        axios.put(`http://localhost:5048/api/UserProfile/${profile.id}`, { ...profile, password })
            .then(() => {
                console.log("Profile saved:", profile);
                if (password) console.log("Password changed to:", password);
                setIsEditing(false);
                setPassword("");
            })
            .catch((error) => console.error("Error saving profile:", error));
    };

    if (!profile) return <p>Loading...</p>; 

    return (
        <div className="container mt-5 d-flex justify-content-center">
            <div className="card shadow-sm p-5" style={{ maxWidth: "500px", background: "#fff", borderRadius: "12px" }}>
                <h3 className="text-center mb-5" style={{ color: "#2c3e50" }}>📝 User Profile</h3>

                {isEditing ? (
                    <>
                        <div className="mb-3">
                            <label className="form-label">Name</label>
                            <input
                                type="text"
                                value={profile.name}
                                onChange={(e) => setProfile({ ...profile, name: e.target.value })}
                                className="form-control"
                            />
                        </div>

                        <div className="mb-3">
                            <label className="form-label">Email</label>
                            <input
                                type="email"
                                value={profile.email}
                                onChange={(e) => setProfile({ ...profile, email: e.target.value })}
                                className="form-control"
                            />
                        </div>

                        <div className="mb-3">
                            <label className="form-label">Phone</label>
                            <input
                                type="text"
                                value={profile.phone}
                                onChange={(e) => setProfile({ ...profile, phone: e.target.value })}
                                className="form-control"
                            />
                        </div>

                        <div className="mb-3">
                            <label className="form-label">New Password</label>
                            <input
                                type="password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                className="form-control"
                                placeholder="Enter new password"
                            />
                        </div>

                        <button onClick={handleSave} className="btn btn-success w-100">Save</button>
                    </>
                ) : (
                    <>
                        <p><strong>Name:</strong> {profile.name}</p>
                        <p><strong>Email:</strong> {profile.email}</p>
                        <p><strong>Phone:</strong> {profile.phone}</p>
                        <button onClick={() => setIsEditing(true)} className="btn btn-primary w-100">Edit Profile</button>
                    </>
                )}
            </div>
        </div>
    );
}
