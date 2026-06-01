import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { Headphones, Eye, EyeOff, CheckCircle } from 'lucide-react'

function SupportIllustration() {
  return (
    <svg viewBox="0 0 480 400" xmlns="http://www.w3.org/2000/svg" fill="none" className="w-full max-w-sm">
      {/* Background decorative blobs */}
      <circle cx="400" cy="60" r="110" fill="#162d58" opacity="0.45" />
      <circle cx="70" cy="360" r="85" fill="#162d58" opacity="0.3" />
      <circle cx="440" cy="340" r="55" fill="#1f3d72" opacity="0.25" />

      {/* Desk */}
      <rect x="60" y="295" width="360" height="18" rx="9" fill="#0d1e3d" />
      <rect x="115" y="313" width="18" height="65" rx="5" fill="#0d1e3d" />
      <rect x="347" y="313" width="18" height="65" rx="5" fill="#0d1e3d" />

      {/* Monitor stand */}
      <rect x="233" y="262" width="14" height="38" rx="4" fill="#162d58" />
      <rect x="210" y="296" width="60" height="8" rx="4" fill="#162d58" />

      {/* Monitor */}
      <rect x="130" y="130" width="220" height="148" rx="12" fill="#0d1e3d" />
      <rect x="140" y="140" width="200" height="122" rx="8" fill="#1f3d72" />

      {/* Screen: header bar */}
      <rect x="140" y="140" width="200" height="28" rx="8" fill="#162d58" />
      <circle cx="160" cy="154" r="9" fill="#3662a7" />
      <path d="M156 150 Q156 159 165 159 L165 156.5 Q163 156.5 161.5 155 L161.5 152.5Z" fill="white" opacity="0.8" />
      <rect x="176" y="149" width="70" height="5" rx="3" fill="#7fa0d0" opacity="0.7" />
      <rect x="176" y="158" width="45" height="4" rx="2" fill="#5580bd" opacity="0.5" />

      {/* Chat messages on screen */}
      <rect x="148" y="177" width="95" height="24" rx="10" fill="#2a4f8c" />
      <rect x="148" y="177" width="95" height="24" rx="10" fill="white" opacity="0.12" />
      <rect x="158" y="184" width="55" height="4" rx="2" fill="white" opacity="0.75" />
      <rect x="158" y="192" width="38" height="4" rx="2" fill="white" opacity="0.5" />

      <rect x="257" y="207" width="75" height="22" rx="10" fill="#3662a7" />
      <rect x="265" y="214" width="45" height="4" rx="2" fill="white" opacity="0.85" />
      <rect x="272" y="222" width="30" height="4" rx="2" fill="white" opacity="0.6" />

      <rect x="148" y="235" width="80" height="22" rx="10" fill="#2a4f8c" />
      <rect x="148" y="235" width="80" height="22" rx="10" fill="white" opacity="0.1" />
      <rect x="158" y="242" width="48" height="4" rx="2" fill="white" opacity="0.7" />
      <rect x="158" y="250" width="32" height="4" rx="2" fill="white" opacity="0.45" />

      {/* Person body */}
      <path d="M165 370 Q165 325 192 315 Q207 308 240 307 Q273 308 288 315 Q315 325 315 370Z" fill="#3662a7" />

      {/* Neck */}
      <rect x="226" y="289" width="28" height="22" rx="6" fill="#FDDCB5" />

      {/* Head */}
      <ellipse cx="240" cy="258" rx="40" ry="44" fill="#FDDCB5" />

      {/* Hair */}
      <path d="M200 248 Q202 208 240 204 Q278 208 280 248 Q272 232 240 229 Q208 232 200 248Z" fill="#2d1b0e" />
      <ellipse cx="200" cy="256" rx="10" ry="20" fill="#2d1b0e" />
      <ellipse cx="280" cy="256" rx="10" ry="20" fill="#2d1b0e" />

      {/* Headset band */}
      <path d="M207 243 Q240 217 273 243" stroke="#0d1e3d" stroke-width="7" stroke-linecap="round" />

      {/* Left earpiece */}
      <rect x="193" y="240" width="20" height="26" rx="8" fill="#162d58" />
      <rect x="197" y="244" width="12" height="18" rx="5" fill="#0d1e3d" />

      {/* Right earpiece */}
      <rect x="267" y="240" width="20" height="26" rx="8" fill="#162d58" />
      <rect x="271" y="244" width="12" height="18" rx="5" fill="#0d1e3d" />

      {/* Mic arm */}
      <path d="M287 255 Q306 265 310 280" stroke="#0d1e3d" stroke-width="4" stroke-linecap="round" />
      <ellipse cx="311" cy="284" rx="7" ry="7" fill="#162d58" />
      <ellipse cx="311" cy="284" rx="3.5" ry="3.5" fill="#3662a7" />

      {/* Eyes */}
      <circle cx="225" cy="256" r="7.5" fill="white" />
      <circle cx="255" cy="256" r="7.5" fill="white" />
      <circle cx="226.5" cy="257" r="5" fill="#2d1b0e" />
      <circle cx="256.5" cy="257" r="5" fill="#2d1b0e" />
      <circle cx="227.5" cy="255" r="1.8" fill="white" />
      <circle cx="257.5" cy="255" r="1.8" fill="white" />

      {/* Smile */}
      <path d="M226 272 Q240 283 254 272" stroke="#c4956a" stroke-width="2.5" stroke-linecap="round" />

      {/* Arms */}
      <path d="M175 340 Q138 335 115 300" stroke="#3662a7" stroke-width="22" stroke-linecap="round" />
      <ellipse cx="108" cy="294" rx="17" ry="12" fill="#FDDCB5" />
      <path d="M305 340 Q342 335 365 300" stroke="#3662a7" stroke-width="22" stroke-linecap="round" />
      <ellipse cx="372" cy="294" rx="17" ry="12" fill="#FDDCB5" />

      {/* Keyboard hints under hands */}
      <rect x="88" y="288" width="44" height="10" rx="3" fill="#162d58" opacity="0.7" />
      <rect x="348" y="288" width="44" height="10" rx="3" fill="#162d58" opacity="0.7" />

      {/* Floating chat bubble - top right */}
      <rect x="365" y="100" width="100" height="58" rx="14" fill="white" opacity="0.1" />
      <rect x="365" y="100" width="100" height="58" rx="14" stroke="white" stroke-width="1.5" opacity="0.25" />
      <rect x="379" y="116" width="58" height="5" rx="3" fill="white" opacity="0.6" />
      <rect x="379" y="127" width="42" height="5" rx="3" fill="white" opacity="0.4" />
      <rect x="379" y="138" width="65" height="5" rx="3" fill="white" opacity="0.35" />
      <path d="M379 158 L365 172 L400 158Z" fill="white" opacity="0.1" />

      {/* Floating notification - top left */}
      <rect x="18" y="110" width="95" height="48" rx="12" fill="white" opacity="0.08" />
      <rect x="18" y="110" width="95" height="48" rx="12" stroke="white" stroke-width="1.5" opacity="0.2" />
      <circle cx="38" cy="134" r="11" fill="#3662a7" opacity="0.7" />
      <rect x="55" y="126" width="45" height="5" rx="3" fill="white" opacity="0.5" />
      <rect x="55" y="135" width="32" height="5" rx="3" fill="white" opacity="0.35" />
      <rect x="55" y="144" width="50" height="5" rx="3" fill="white" opacity="0.3" />

      {/* Decorative dots */}
      <circle cx="430" cy="270" r="5" fill="white" opacity="0.18" />
      <circle cx="447" cy="284" r="3.5" fill="white" opacity="0.12" />
      <circle cx="438" cy="300" r="6" fill="white" opacity="0.1" />
      <circle cx="55" cy="200" r="4" fill="white" opacity="0.14" />
      <circle cx="70" cy="216" r="3" fill="white" opacity="0.1" />

      {/* Signal waves */}
      <path d="M418 42 Q427 31 436 42" stroke="#7fa0d0" stroke-width="3" stroke-linecap="round" opacity="0.65" />
      <path d="M411 42 Q425 25 437 42" stroke="#7fa0d0" stroke-width="3" stroke-linecap="round" opacity="0.45" />
      <path d="M404 42 Q423 18 439 42" stroke="#7fa0d0" stroke-width="3" stroke-linecap="round" opacity="0.28" />
      <circle cx="427" cy="48" r="4" fill="#7fa0d0" opacity="0.75" />
    </svg>
  )
}

const features = [
  'Pregled i praćenje vaših aktivnosti u realnom vremenu',
  'Obavijesti o svim promjenama i ažuriranjima',
  'Sigurna i pouzdana platforma za sve korisnike',
]

export default function Login() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [showPassword, setShowPassword] = useState(false)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e) {
    e.preventDefault()
    if (!email || !password) return
    setError('')
    setLoading(true)
    try {
      await login(email, password)
      navigate('/dashboard')
    } catch {
      setError('Nevažeći podaci. Molimo pokušajte ponovo.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen flex">
      {/* Left panel — login form */}
      <div className="flex-1 flex items-center justify-center p-8 lg:p-16 bg-navy-50">
        <div className="w-full max-w-md">
          {/* Brand */}
          <div className="mb-10">
            <div className="inline-flex items-center justify-center w-14 h-14 rounded-2xl bg-navy-700 mb-5 shadow-lg">
              <Headphones size={28} className="text-white" />
            </div>
            <div className="flex items-baseline gap-2 mb-1">
              <h1 className="text-2xl font-bold text-navy-900 tracking-tight">TelecomSupport</h1>
            </div>
            <p className="text-navy-400 text-sm">Sistem za upravljanje tiketima</p>
          </div>

          {/* Heading */}
          <h2 className="text-3xl font-bold text-gray-900 mb-2">Dobrodošli nazad</h2>
          <p className="text-gray-500 text-sm mb-8">
            Prijavite se da nastavite na vaš radni prostor.
          </p>

          {error && (
            <div className="mb-5 p-3.5 bg-red-50 border border-red-200 rounded-xl text-sm text-red-700">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-5">
            <div>
              <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-1.5">
                Email adresa
              </label>
              <input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                className="w-full px-4 py-3 border border-gray-200 rounded-xl text-sm bg-gray-50 focus:bg-white focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none transition-all placeholder:text-gray-400"
                placeholder="you@example.com"
              />
            </div>

            <div>
              <label htmlFor="password" className="block text-sm font-medium text-gray-700 mb-1.5">
                Lozinka
              </label>
              <div className="relative">
                <input
                  id="password"
                  type={showPassword ? 'text' : 'password'}
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                  className="w-full px-4 py-3 border border-gray-200 rounded-xl text-sm bg-gray-50 focus:bg-white focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none transition-all pr-11 placeholder:text-gray-400"
                  placeholder="Unesite vašu lozinku"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3.5 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition-colors"
                >
                  {showPassword ? <EyeOff size={16} /> : <Eye size={16} />}
                </button>
              </div>
            </div>

            <button
              type="submit"
              disabled={loading}
              className="w-full py-3 px-4 bg-navy-700 hover:bg-navy-800 active:bg-navy-900 text-white font-semibold rounded-xl text-sm transition-all disabled:opacity-50 shadow-sm hover:shadow-md mt-2"
            >
              {loading ? 'Prijavljivanje...' : 'Prijavi se'}
            </button>
          </form>
        </div>
      </div>

      {/* Right panel — illustration (hidden on mobile/tablet) */}
      <div className="hidden lg:flex flex-1 bg-gradient-to-br from-navy-800 via-navy-850 to-navy-950 relative overflow-hidden items-center justify-center p-12">
        {/* Background decorative circles */}
        <div className="absolute -top-20 -right-20 w-72 h-72 bg-navy-700 rounded-full opacity-30" />
        <div className="absolute -bottom-16 -left-16 w-56 h-56 bg-navy-600 rounded-full opacity-20" />
        <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[500px] h-[500px] bg-navy-700 rounded-full opacity-10" />

        {/* Content */}
        <div className="relative z-10 flex flex-col items-center text-center max-w-md">
          <SupportIllustration />

          <h3 className="text-white text-2xl font-bold mt-8 mb-3">
            Sve na jednom mjestu
          </h3>
          <p className="text-navy-300 text-sm leading-relaxed mb-8">
            Jednostavan pristup vašim zahtjevima, tiketima i komunikaciji.
          </p>

          <div className="flex flex-col gap-3 w-full text-left">
            {features.map((f) => (
              <div key={f} className="flex items-center gap-3 text-navy-200 text-sm">
                <CheckCircle size={16} className="text-navy-400 shrink-0" />
                {f}
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  )
}
