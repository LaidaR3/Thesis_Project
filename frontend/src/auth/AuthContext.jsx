import { createContext, useContext, useEffect, useState }  from "react";
import { jwtDecode } from "jwt-decode";

const AuthContext = createContext (null);

export function AuthProvider({ children }) {
    const [user, setUser] = useState(null);

    useEffect(() => {
        const token = sessionStorage.getItem("token");

        if (token) {
            try{
                const decoded = jwtDecode(token);

                setUser({
                    id:decoded.sub,
                    email: decoded.email,
                    roles: decoded.role || decoded.roles,
                });
            }catch{
               sessionStorage.removeItem("token");
               setUser(null); 
            }
        }
    }, []);

    return (
        <AuthContext.Provider value={{ user, setUser }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    return useContext(AuthContext);
}