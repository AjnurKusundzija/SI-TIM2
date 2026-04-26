import { createContext, useContext, useMemo, useState } from 'react'
import PropTypes from 'prop-types'
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

  async function logout() {
    await logoutService()
    setUser(null)
  }

  const value = useMemo(() => ({ user, login, logout }), [user])

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  )
}

AuthProvider.propTypes = {
  children: PropTypes.node.isRequired,
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
  return useContext(AuthContext)
}
