import { useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { Ticket, PlusCircle, LayoutDashboard } from 'lucide-react'

function QuickCard({ icon: Icon, label, description, to, color }) {
  const navigate = useNavigate()
  return (
    <div
      onClick={() => navigate(to)}
      className="bg-white rounded-xl p-5 shadow-sm border border-gray-100 cursor-pointer hover:shadow-md transition-shadow"
    >
      <div className="flex items-center gap-4">
        <div className={`w-12 h-12 rounded-xl flex items-center justify-center ${color}`}>
          <Icon size={22} className="text-white" />
        </div>
        <div>
          <p className="text-sm font-semibold text-gray-900">{label}</p>
          <p className="text-xs text-gray-500 mt-0.5">{description}</p>
        </div>
      </div>
    </div>
  )
}

export default function Dashboard() {
  const { user } = useAuth()

  const clientCards = [
    { icon: Ticket, label: 'My Tickets', description: 'View and track your support tickets', to: '/mytickets', color: 'bg-navy-600' },
    { icon: PlusCircle, label: 'Create Ticket', description: 'Submit a new support request', to: '/create-ticket', color: 'bg-emerald-500' },
  ]

  const staffCards = [
    { icon: Ticket, label: 'Tickets', description: 'View and manage support tickets', to: '/tickets', color: 'bg-navy-600' },
  ]

  const cards = user?.role === 'CLIENT' ? clientCards : staffCards

  const roleDescription = {
    CLIENT: 'Here is an overview of your support account.',
    AGENT: 'Here is your ticket queue and activity summary.',
    TECHNICIAN: 'Here are your assigned tickets and tasks.',
    ADMINISTRATOR: 'Here is your system-wide overview.',
  }

  return (
    <div className="space-y-6">
      {/* Welcome banner */}
      <div className="bg-gradient-to-r from-navy-800 to-navy-700 rounded-xl p-6 text-white">
        <h2 className="text-xl font-bold">
          Welcome back, {user?.firstName}!
        </h2>
        <p className="text-navy-200 text-sm mt-1">
          {roleDescription[user?.role] || 'Welcome to TelecomSupport.'}
        </p>
      </div>

      {/* Quick action cards */}
      <div>
        <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">
          Quick Actions
        </h3>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          {cards.map((card) => (
            <QuickCard key={card.to} {...card} />
          ))}
        </div>
      </div>
    </div>
  )
}
