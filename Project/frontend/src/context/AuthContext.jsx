import { createContext, useContext, useState } from 'react'
import { login as loginService, logout as logoutService, getUser } from '../services/authService'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [user, setUser] = useState(getUser)

  async function login(email, password) {
    const data = await loginService(email, password)
    setUser({
      userId: data.userId,
      firstName: data.firstName,
      lastName: data.lastName,
      email: data.email,
      role: data.role,
    })
  }

  function logout() {
    logoutService()
    setUser(null)
  }

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  return useContext(AuthContext)
}
