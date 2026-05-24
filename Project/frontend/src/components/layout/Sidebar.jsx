import PropTypes from 'prop-types';
import { useState } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { useNotifications } from '../../context/NotificationContext';
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
  UserMinus,
  ChevronDown,
  ChevronRight
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
  { to: '/faq', label: 'FAQ', icon: HelpCircle },
  { to: '/notifications', label: 'Notifikacije', icon: Bell },
],

};

export default function Sidebar({ isOpen, onClose }) {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const { unreadCount } = useNotifications();
  const links = navConfig[user?.role] || [];
  
  const [openMenus, setOpenMenus] = useState({});

  const toggleMenu = (label) => {
    setOpenMenus(prev => ({
      ...prev,
      [label]: !prev[label]
    }));
  };

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  return (
    <>
      {isOpen && (
        <div
          className="fixed inset-0 bg-black/50 z-40 lg:hidden"
          onClick={onClose}
        />
      )}

      <aside
        className={`fixed top-0 left-0 z-50 h-full w-64 bg-navy-900 text-white flex flex-col transition-transform duration-300 lg:translate-x-0 ${
          isOpen ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        {/* Logo */}
        <div className="flex items-center justify-between px-5 py-5 border-b border-navy-700">
          <div className="flex items-center gap-2">
            <Headphones size={24} className="text-navy-300" />
            <span className="text-lg font-bold tracking-tight">TelecomSupport</span>
          </div>
          <button onClick={onClose} className="lg:hidden text-navy-300 hover:text-white">
            <X size={20} />
          </button>
        </div>

        {/* User info */}
        <div className="px-5 py-4 border-b border-navy-700">
          <div className="flex items-center gap-3">
            <div className="w-9 h-9 rounded-full bg-navy-600 flex items-center justify-center text-sm font-medium">
              {user?.firstName?.[0]}{user?.lastName?.[0]}
            </div>
            <div className="min-w-0">
              <p className="text-sm font-medium truncate">
                {user?.firstName} {user?.lastName}
              </p>
              <p className="text-xs text-navy-300 truncate">{user?.role}</p>
            </div>
          </div>
        </div>

        {/* Navigation */}
        <nav className="flex-1 overflow-y-auto py-4 px-3">
          {links.map((link) => {
            if (link.subItems) {
              const isOpen = openMenus[link.label];
              return (
                <div key={link.label} className="mb-1">
                  <button
                    onClick={() => toggleMenu(link.label)}
                    className={`w-full flex items-center justify-between px-3 py-2.5 rounded-lg text-sm font-medium transition-colors ${
                      isOpen ? 'bg-navy-800 text-white' : 'text-navy-300 hover:bg-navy-800 hover:text-white'
                    }`}
                  >
                    <div className="flex items-center gap-3">
                      <link.icon size={18} />
                      <span>{link.label}</span>
                    </div>
                    {isOpen ? <ChevronDown size={16} /> : <ChevronRight size={16} />}
                  </button>
                  
                  {isOpen && (
                    <div className="mt-1 ml-9 space-y-1">
                      {link.subItems.map(subItem => (
                        <NavLink
                          key={subItem.to}
                          to={subItem.to}
                          onClick={onClose}
                          className={({ isActive }) =>
                            `block px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
                              isActive
                                ? 'bg-navy-700 text-white'
                                : 'text-navy-400 hover:bg-navy-800 hover:text-white'
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
                  `flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-colors mb-1 ${
                    isActive
                      ? 'bg-navy-700 text-white'
                      : 'text-navy-300 hover:bg-navy-800 hover:text-white'
                  }`
                }
              >
                <link.icon size={18} />
                <span className="flex-1">{link.label}</span>
                {link.to === '/notifications' && unreadCount > 0 && (
                  <span className="min-w-[18px] h-[18px] px-1 bg-red-500 text-white text-[10px] font-bold rounded-full flex items-center justify-center leading-none">
                    {unreadCount > 9 ? '9+' : unreadCount}
                  </span>
                )}
              </NavLink>
            );
          })}
        </nav>

        {/* Logout */}
        <div className="px-3 py-4 border-t border-navy-700">
          <button
            onClick={handleLogout}
            className="flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium text-navy-300 hover:bg-navy-800 hover:text-white transition-colors w-full"
          >
            <LogOut size={18} />
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
