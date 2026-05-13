import PropTypes from 'prop-types';

const variants = {
  // Status badges
  OPEN: 'bg-emerald-100 text-emerald-800',
  CLOSED: 'bg-gray-100 text-gray-800',
  PENDING_CLOSE: 'bg-amber-100 text-amber-800',
  CLOSURE_REQUESTED: 'bg-amber-100 text-amber-800',
  // Priority badges
  LOW: 'bg-blue-100 text-blue-800',
  MEDIUM: 'bg-yellow-100 text-yellow-800',
  HIGH: 'bg-red-100 text-red-800',
  CRITICAL: 'bg-red-900 text-red-100',
  // Category badges
  INTERNET: 'bg-indigo-100 text-indigo-800',
  TV: 'bg-purple-100 text-purple-800',
  MOBILE_NETWORK: 'bg-pink-100 text-pink-800',
  BILLING: 'bg-orange-100 text-orange-800',
  TECHNICAL_SUPPORT: 'bg-teal-100 text-teal-800',
  // Role badges
  CLIENT: 'bg-sky-100 text-sky-800',
  AGENT: 'bg-violet-100 text-violet-800',
  TECHNICIAN: 'bg-lime-100 text-lime-800',
  ADMINISTRATOR: 'bg-rose-100 text-rose-800',
  // Account status
  ACTIVE: 'bg-emerald-100 text-emerald-800',
  INACTIVE: 'bg-red-100 text-red-800',
  // Default
  default: 'bg-gray-100 text-gray-800',
};

const labels = {
  PENDING_CLOSE: 'ČEKA SE',
  CLOSURE_REQUESTED: 'ČEKA SE',
  MOBILE_NETWORK: 'Mobilna mreža',
  TECHNICAL_SUPPORT: 'Tehnička podrška',
  // Status badges
  OPEN: 'OTVOREN',
  CLOSED: 'ZATVOREN',
  // Priority badges
  LOW: 'NIZAK',
  MEDIUM: 'SREDNJI',
  HIGH: 'VISOK',
  CRITICAL: 'KRITICAN',
  // Role badges
  CLIENT: 'KLIJENT',
  AGENT: 'AGENT',
  TECHNICIAN: 'TEHNICAR',
  ADMINISTRATOR: 'ADMINISTRATOR',
  // Account status
  ACTIVE: 'AKTIVAN',
  INACTIVE: 'NEAKTIVAN',
};

export default function Badge({ value, className = '' }) {
  const colorClass = variants[value] || variants.default;
  const label = labels[value] || value;

  return (
    <span
      className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${colorClass} ${className}`}
    >
      {label}
    </span>
  );
}

Badge.propTypes = {
  value: PropTypes.string.isRequired,
  className: PropTypes.string,
};
