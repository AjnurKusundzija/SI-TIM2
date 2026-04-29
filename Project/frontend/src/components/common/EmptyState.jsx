import PropTypes from 'prop-types';
import { Inbox } from 'lucide-react';

export default function EmptyState({
  icon: Icon = Inbox,
  title = 'Nema podataka',
  description = 'Trenutno nema ništa za prikaz.',
  action,
  actionLabel,
}) {
  // Ensure linters always treat Icon as used (some configs miss JSX usage)
  void Icon;
  return (
    <div className="flex flex-col items-center justify-center py-16 px-4">
      <div className="w-16 h-16 rounded-full bg-gray-100 flex items-center justify-center mb-4">
        <Icon size={28} className="text-gray-400" />
      </div>
      <h3 className="text-lg font-medium text-gray-900 mb-1">{title}</h3>
      <p className="text-sm text-gray-500 text-center max-w-sm mb-4">{description}</p>
      {action && actionLabel && (
        <button
          onClick={action}
          className="px-4 py-2 text-sm font-medium text-white bg-navy-700 rounded-lg hover:bg-navy-800 transition-colors"
        >
          {actionLabel}
        </button>
      )}
    </div>
  );
}

EmptyState.propTypes = {
  icon: PropTypes.elementType,
  title: PropTypes.string,
  description: PropTypes.string,
  action: PropTypes.func,
  actionLabel: PropTypes.string,
};
