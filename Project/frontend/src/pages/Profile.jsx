import { useEffect, useState } from 'react'
import { useAuth } from '../context/AuthContext'
import { getMyProfile, updateEmail, updatePassword } from '../services/userService'

function getErrorMessage(error) {
  return (
    error?.response?.data?.message || error?.message || 'Došlo je do greške. Pokušajte ponovno.'
  )
}

export default function Profile() {
  const { user, updateUser } = useAuth()
  const [profile, setProfile] = useState({
    firstName: user?.firstName || '',
    lastName: user?.lastName || '',
    email: user?.email || '',
    phone: '',
    role: user?.role || '',
  })
  const [newEmail, setNewEmail] = useState(user?.email || '')
  const [emailStatus, setEmailStatus] = useState(null)
  const [emailError, setEmailError] = useState(null)
  const [currentPassword, setCurrentPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [passwordStatus, setPasswordStatus] = useState(null)
  const [passwordError, setPasswordError] = useState(null)

  useEffect(() => {
    async function loadProfile() {
      try {
        const data = await getMyProfile()
        setProfile(data)
        setNewEmail(data.email)
      } catch (error) {
        setEmailError(getErrorMessage(error))
      }
    }

    loadProfile()
  }, [])

  const handleEmailSubmit = async (event) => {
    event.preventDefault()
    setEmailStatus(null)
    setEmailError(null)

    // 1. Provjera da li je polje prazno
    if (!newEmail.trim()) {
      setEmailError("Email je obavezan.")
      return
    }

    // 2. Provjera formata (mora imati @ i završavati na .com)
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.com$/
    if (!emailRegex.test(newEmail)) {
      setEmailError("Neispravan format email adrese. Mora sadržavati '@' i završavati se sa '.com'.")
      return
    }

    // 3. Slanje na backend (provjera zauzetosti u bazi)
    try {
      await updateEmail(newEmail)
      setEmailStatus('Email je uspješno ažuriran.')
      setProfile((prev) => ({ ...prev, email: newEmail }))
      updateUser({ email: newEmail })
    } catch (error) {
      setEmailError(getErrorMessage(error))
    }
  }

  const handlePasswordSubmit = async (event) => {
    event.preventDefault()
    setPasswordStatus(null)
    setPasswordError(null)

    if (newPassword !== confirmPassword) {
      setPasswordError('Lozinke se ne poklapaju.')
      return
    }

    try {
      await updatePassword(currentPassword, newPassword, confirmPassword)
      setPasswordStatus('Lozinka je uspješno promijenjena.')
      setCurrentPassword('')
      setNewPassword('')
      setConfirmPassword('')
    } catch (error) {
      setPasswordError(getErrorMessage(error))
    }
  }

  return (
    <div className="min-h-[calc(100vh-4rem)] px-6 py-8 lg:px-10">
      <div className="max-w-4xl mx-auto space-y-8">
        <header className="rounded-3xl bg-white p-8 shadow-sm border border-slate-200">
          <div className="flex flex-col gap-2">
            <p className="text-sm uppercase tracking-[0.24em] text-navy-600 font-semibold">
              Profil
            </p>
            <h1 className="text-3xl font-semibold text-navy-900">Podaci o korisniku</h1>
            <p className="max-w-2xl text-sm text-slate-500">
              Ovdje možete pregledati svoj profil i promijeniti email adresu ili lozinku.
            </p>
          </div>
        </header>

        <section className="grid gap-8 lg:grid-cols-2">
          <div className="rounded-3xl bg-white p-8 shadow-sm border border-slate-200">
            <p className="text-sm font-semibold text-navy-700 mb-6">Osnovni podaci</p>
            <div className="space-y-4 text-sm text-slate-700">
              <div>
                <p className="text-xs uppercase tracking-[0.24em] text-slate-400">Ime</p>
                <p>{profile.firstName}</p>
              </div>
              <div>
                <p className="text-xs uppercase tracking-[0.24em] text-slate-400">Prezime</p>
                <p>{profile.lastName}</p>
              </div>
              <div>
                <p className="text-xs uppercase tracking-[0.24em] text-slate-400">Email</p>
                <p>{profile.email}</p>
              </div>
              {/* ULOGA JE USPJEŠNO UKLONJENA ODAVDE */}
              {profile.phone && (
                <div>
                  <p className="text-xs uppercase tracking-[0.24em] text-slate-400">Telefon</p>
                  <p>{profile.phone}</p>
                </div>
              )}
            </div>
          </div>

          <div className="rounded-3xl bg-white p-8 shadow-sm border border-slate-200">
            <p className="text-sm font-semibold text-navy-700 mb-6">Ažuriranje profila</p>
            <form onSubmit={handleEmailSubmit} className="space-y-4">
              <label className="block text-sm font-medium text-slate-700">
                Nova email adresa
              </label>
              <input
                type="email"
                value={newEmail}
                onChange={(e) => setNewEmail(e.target.value)}
                className="w-full rounded-2xl border border-slate-300 px-4 py-3 text-sm focus:border-navy-500 focus:outline-none focus:ring-2 focus:ring-navy-200"
                placeholder="unesi novu email adresu"
                required
              />
              {emailError && (
                <p className="text-sm text-red-600">{emailError}</p>
              )}
              {emailStatus && (
                <p className="text-sm text-green-600">{emailStatus}</p>
              )}
              <button
                type="submit"
                className="inline-flex items-center justify-center rounded-2xl bg-navy-700 px-5 py-3 text-sm font-semibold text-white transition hover:bg-navy-800"
              >
                Sačuvaj email
              </button>
            </form>
          </div>
        </section>

        <section className="rounded-3xl bg-white p-8 shadow-sm border border-slate-200">
          <p className="text-sm font-semibold text-navy-700 mb-6">Promjena lozinke</p>
          <form onSubmit={handlePasswordSubmit} className="space-y-5">
            <div>
              <label className="block text-sm font-medium text-slate-700">Trenutna lozinka</label>
              <input
                type="password"
                value={currentPassword}
                onChange={(e) => setCurrentPassword(e.target.value)}
                className="w-full rounded-2xl border border-slate-300 px-4 py-3 text-sm focus:border-navy-500 focus:outline-none focus:ring-2 focus:ring-navy-200"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700">Nova lozinka</label>
              <input
                type="password"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                className="w-full rounded-2xl border border-slate-300 px-4 py-3 text-sm focus:border-navy-500 focus:outline-none focus:ring-2 focus:ring-navy-200"
                minLength={8}
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-slate-700">Potvrdite novu lozinku</label>
              <input
                type="password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                className="w-full rounded-2xl border border-slate-300 px-4 py-3 text-sm focus:border-navy-500 focus:outline-none focus:ring-2 focus:ring-navy-200"
                minLength={8}
                required
              />
            </div>
            {passwordError && (
              <p className="text-sm text-red-600">{passwordError}</p>
            )}
            {passwordStatus && (
              <p className="text-sm text-green-600">{passwordStatus}</p>
            )}
            <button
              type="submit"
              className="inline-flex items-center justify-center rounded-2xl bg-navy-700 px-5 py-3 text-sm font-semibold text-white transition hover:bg-navy-800"
            >
              Promijeni lozinku
            </button>
          </form>
        </section>
      </div>
    </div>
  )
}