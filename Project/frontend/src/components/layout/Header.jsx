import PropTypes from 'prop-types';
import { Menu, Bell } from 'lucide-react';

export default function Header({ onMenuToggle, title }) {
  return (
    <header className="sticky top-0 z-30 bg-white border-b border-gray-200 px-4 lg:px-6 py-3">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <button
            onClick={onMenuToggle}
            className="lg:hidden p-2 text-gray-500 hover:text-gray-700 rounded-lg hover:bg-gray-100"
          >
            <Menu size={20} />
          </button>
          <h1 className="text-lg font-semibold text-gray-900">{title}</h1>
        </div>

        <div className="flex items-center gap-3">
          <button className="relative p-2 text-gray-500 hover:text-gray-700 rounded-lg hover:bg-gray-100">
            <Bell size={20} />
          </button>
        </div>
      </div>
    </header>
  );
}

Header.propTypes = {
  onMenuToggle: PropTypes.func.isRequired,
  title: PropTypes.string.isRequired,
};
