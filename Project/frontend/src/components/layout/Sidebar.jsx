import PropTypes from 'prop-types';
import { useState } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { useNotifications } from '../../context/NotificationContext';
import { useUIStore } from '../../store/uiStore';
import {
  LayoutDashboard,
  Ticket,
  LogOut,
  X,
  Headphones,
  PlusCircle,
  HelpCircle,
  BarChart2,
  Bell,
  Package,
  User,
  Users,
  Users2,
  ChevronDown,
  ChevronRight,
  History,
  AlertTriangle,
} from 'lucide-react';

const navConfig = {
  CLIENT: [
    { to: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
    { to: '/profile', label: 'Profil', icon: User },
    { to: '/mytickets', label: 'Moji tiketi', icon: Ticket },
    { to: '/create-ticket', label: 'Kreiraj tiket', icon: PlusCircle },
    { to: '/packages', label: 'Moji paketi', icon: Package },
    { to: '/faq', label: 'FAQ', icon: HelpCircle },
    { to: '/notifications', label: 'Notifikacije', icon: Bell },
  ],
  AGENT: [
    { to: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
    { to: '/profile', label: 'Profil', icon: User },
    { to: '/tickets', label: 'Svi tiketi', icon: Ticket },
    { to: '/assigned', label: 'Dodijeljeni meni', icon: Ticket },
    {
      label: 'Korisnici',
      icon: Users,
      subItems: [
        { to: '/users/clients', label: 'Klijenti' },
        { to: '/users/technicians', label: 'Tehničari' },
      ],
    },
    { to: '/statistics', label: 'Moja statistika', icon: BarChart2 },
    { to: '/faq', label: 'FAQ', icon: HelpCircle },
    { to: '/notifications', label: 'Notifikacije', icon: Bell },
  ],
  TECHNICIAN: [
    { to: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
    { to: '/profile', label: 'Profil', icon: User },
    { to: '/assigned', label: 'Dodijeljeni meni', icon: Ticket },
    { to: '/statistics', label: 'Moja statistika', icon: BarChart2 },
    { to: '/notifications', label: 'Notifikacije', icon: Bell },
  ],
  ADMINISTRATOR: [
    { to: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
    { to: '/profile', label: 'Profil', icon: User },
    { to: '/reports', label: 'Izvještaji', icon: BarChart2 },
    { to: '/tickets', label: 'Svi tiketi', icon: Ticket },
    {
      label: 'Korisnici',
      icon: Users,
      subItems: [
        { to: '/users/clients', label: 'Klijenti' },
        { to: '/users/agents', label: 'Agenti' },
        { to: '/users/technicians', label: 'Tehničari' },
        { to: '/users/deactivated', label: 'Deaktivirani' },
      ],
    },
    { to: '/admin/packages', label: 'Upravljanje paketima', icon: Package },
    { to: '/teams', label: 'Timovi', icon: Users2 },
    { to: '/audit-log', label: 'Audit log', icon: History },
    { to: '/faq', label: 'FAQ', icon: HelpCircle },
    { to: '/notifications', label: 'Notifikacije', icon: Bell },
  ],
};

const ROLE_LABELS = {
  CLIENT: 'Klijent',
  AGENT: 'Agent',
  TECHNICIAN: 'Tehničar',
  ADMINISTRATOR: 'Administrator',
};

export default function Sidebar({ isOpen, onClose }) {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const { unreadCount } = useNotifications();
  const { alertTicketCount, alertTicketUrl } = useUIStore();
  const isAdmin = user?.role === 'ADMINISTRATOR';
  const links = navConfig[user?.role] || [];

  const [openMenus, setOpenMenus] = useState({});

  const toggleMenu = (label) => {
    setOpenMenus(prev => ({ ...prev, [label]: !prev[label] }));
  };

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  const initials = `${user?.firstName?.[0] ?? ''}${user?.lastName?.[0] ?? ''}`.toUpperCase();

  return (
    <>
      {isOpen && (
        <div
          className="fixed inset-0 bg-black/40 backdrop-blur-sm z-40 lg:hidden"
          onClick={onClose}
        />
      )}

      <aside
        className={`fixed top-0 left-0 z-50 h-full w-60 bg-[#f0f2f5] border-r border-slate-200 flex flex-col transition-transform duration-300 lg:translate-x-0 ${
          isOpen ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        {/* Logo */}
        <div className="flex items-center justify-between px-5 h-16 border-b border-slate-200 flex-shrink-0">
          <div className="flex items-center gap-2.5">
            <div className="w-8 h-8 rounded-lg bg-navy-800 flex items-center justify-center flex-shrink-0">
              <Headphones size={16} className="text-white" />
            </div>
            <span className="text-sm font-bold text-gray-900 tracking-tight">TelecomSupport</span>
          </div>
          <button onClick={onClose} className="lg:hidden p-1 text-gray-400 hover:text-gray-600 rounded-md">
            <X size={18} />
          </button>
        </div>

        {/* User info */}
        <div className="px-4 py-4 border-b border-slate-200 flex-shrink-0">
          <div className="flex items-center gap-3">
            <div className="w-9 h-9 rounded-full bg-navy-800 flex items-center justify-center text-xs font-semibold text-white flex-shrink-0">
              {initials || '?'}
            </div>
            <div className="min-w-0">
              <p className="text-sm font-semibold text-gray-900 truncate leading-tight">
                {user?.firstName} {user?.lastName}
              </p>
              <p className="text-xs text-gray-400 truncate mt-0.5">
                {ROLE_LABELS[user?.role] ?? user?.role}
              </p>
            </div>
          </div>
        </div>

        {/* Navigation */}
        <nav className="flex-1 overflow-y-auto py-3 px-3 space-y-0.5">
          {links.map((link) => {
            if (link.subItems) {
              const expanded = openMenus[link.label];
              return (
                <div key={link.label}>
                  <button
                    onClick={() => toggleMenu(link.label)}
                    className={`w-full flex items-center justify-between px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
                      expanded
                        ? 'bg-navy-50 text-navy-800'
                        : 'text-gray-600 hover:bg-slate-100 hover:text-gray-900'
                    }`}
                  >
                    <div className="flex items-center gap-3">
                      <link.icon size={17} className={expanded ? 'text-navy-700' : 'text-gray-400'} />
                      <span>{link.label}</span>
                    </div>
                    {expanded
                      ? <ChevronDown size={14} className="text-navy-400" />
                      : <ChevronRight size={14} className="text-gray-300" />
                    }
                  </button>

                  {expanded && (
                    <div className="mt-0.5 ml-8 space-y-0.5 mb-1">
                      {link.subItems.map(subItem => (
                        <NavLink
                          key={subItem.to}
                          to={subItem.to}
                          onClick={onClose}
                          className={({ isActive }) =>
                            `block px-3 py-1.5 rounded-md text-sm transition-colors ${
                              isActive
                                ? 'text-navy-800 font-semibold bg-navy-50'
                                : 'text-gray-500 hover:text-gray-800 hover:bg-slate-100'
                            }`
                          }
                        >
                          {subItem.label}
                        </NavLink>
                      ))}
                    </div>
                  )}
                </div>
              );
            }

            return (
              <NavLink
                key={link.to}
                to={link.to}
                onClick={onClose}
                className={({ isActive }) =>
                  `flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
                    isActive
                      ? 'bg-navy-50 text-navy-800'
                      : 'text-gray-600 hover:bg-slate-100 hover:text-gray-900'
                  }`
                }
              >
                {({ isActive }) => (
                  <>
                    <link.icon
                      size={17}
                      className={isActive ? 'text-navy-700' : 'text-gray-400'}
                    />
                    <span className="flex-1">{link.label}</span>
                    {link.to === '/notifications' && unreadCount > 0 && (
                      <span className="min-w-[18px] h-[18px] px-1 bg-navy-800 text-white text-[10px] font-bold rounded-full flex items-center justify-center leading-none">
                        {unreadCount > 9 ? '9+' : unreadCount}
                      </span>
                    )}
                  </>
                )}
              </NavLink>
            );
          })}
        </nav>

        {/* Bottom status — admin only */}
        {isAdmin && (
          <div className="px-4 py-3 border-t border-slate-200 flex-shrink-0">
            {alertTicketCount > 0 && alertTicketUrl ? (
              <button
                onClick={() => { navigate(alertTicketUrl); onClose(); }}
                className="w-full flex items-center gap-2.5 px-3 py-2.5 rounded-xl bg-amber-50 border border-amber-100 hover:bg-amber-100 transition-colors text-left"
              >
                <AlertTriangle size={14} className="text-amber-500 flex-shrink-0" />
                <div className="min-w-0 flex-1">
                  <p className="text-xs font-semibold text-amber-700 leading-tight">
                    {alertTicketCount} tiketa zahtijeva pažnju
                  </p>
                  <p className="text-[10px] text-amber-500 mt-0.5">Klikni za pregled →</p>
                </div>
              </button>
            ) : (
              <div className="flex items-center gap-2.5 px-3 py-2.5 rounded-xl bg-emerald-50 border border-emerald-100">
                <span className="w-2 h-2 rounded-full bg-emerald-400 flex-shrink-0 animate-pulse" />
                <div className="min-w-0">
                  <p className="text-xs font-semibold text-emerald-700 leading-tight">Sistem aktivan</p>
                  <p className="text-[10px] text-emerald-500 mt-0.5">Nema tiketa na čekanju</p>
                </div>
              </div>
            )}
          </div>
        )}

        {/* Logout */}
        <div className="px-3 py-3 border-t border-slate-200 flex-shrink-0">
          <button
            onClick={handleLogout}
            className="flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium text-gray-500 hover:bg-red-50 hover:text-red-600 transition-colors w-full"
          >
            <LogOut size={17} className="text-gray-400" />
            Odjavi se
          </button>
        </div>
      </aside>
    </>
  );
}

Sidebar.propTypes = {
  isOpen: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
};
