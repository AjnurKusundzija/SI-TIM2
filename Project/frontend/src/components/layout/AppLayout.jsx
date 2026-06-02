import { useState } from 'react';
import { Outlet, useLocation } from 'react-router-dom';
import Sidebar from './Sidebar';
import Header from './Header';

const pageTitles = {
  '/dashboard': 'Dashboard',
  '/mytickets': 'Moji tiketi',
  '/tickets': 'Tiketi',
  '/create-ticket': 'Kreiraj tiket',
  '/faq': 'FAQ',
  '/reports': 'Izvještaji',
  '/assigned': 'Dodijeljeni meni',
  '/profile': 'Profil',
  '/statistics': 'Statistika',
  '/notifications': 'Notifikacije',
  '/packages': 'Moji paketi',
  '/admin/packages': 'Upravljanje paketima',
  '/audit-log': 'Audit log',
  '/users/clients': 'Klijenti',
  '/users/agents': 'Agenti',
  '/users/technicians': 'Tehničari',
  '/users/deactivated': 'Deaktivirani korisnici',
};

export default function AppLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const location = useLocation();

  const getTitle = () => {
    for (const [path, title] of Object.entries(pageTitles)) {
      if (location.pathname === path || location.pathname.startsWith(path + '/')) {
        return title;
      }
    }
    return 'TelecomSupport';
  };

  return (
    <div className="min-h-screen bg-[#f4f6f9]">
      <Sidebar isOpen={sidebarOpen} onClose={() => setSidebarOpen(false)} />
      <div className="lg:ml-60 flex flex-col min-h-screen">
        <Header
          onMenuToggle={() => setSidebarOpen(!sidebarOpen)}
          title={getTitle()}
        />
        <main className="flex-1 p-4 lg:p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
