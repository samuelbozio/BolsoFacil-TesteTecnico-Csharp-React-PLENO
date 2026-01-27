import React from 'react';
import LoggerService from '@/services/logger';

interface NotificationProps {
  id: string;
  message: string;
  type: 'success' | 'error' | 'warning' | 'info';
  onClose: (id: string) => void;
  duration?: number;
}

const Notification: React.FC<NotificationProps> = (props: NotificationProps) => {
  const { id, message, type, onClose, duration = 5000 } = props;
  
  React.useEffect(() => {
    LoggerService.debug(
      `Notificação criada: ${message}`,
      'NOTIFICATION',
      { id, type, duration }
    );

    if (duration > 0) {
      const timer = setTimeout(() => {
        LoggerService.debug(`Notificação ${id} fechando automaticamente`, 'NOTIFICATION');
        onClose(id);
      }, duration);
      return () => clearTimeout(timer);
    }
  }, [id, duration, onClose, message, type]);

  const handleClose = () => {
    LoggerService.debug(`Notificação ${id} fechada pelo usuário`, 'NOTIFICATION');
    onClose(id);
  };

  return (
    <div className={`alert alert-${type}`} style={{ marginBottom: '0.75rem' }}>
      <span>{message}</span>
      <button
        className="close-btn"
        onClick={handleClose}
        style={{ marginLeft: 'auto', padding: '0' }}
      >
        ×
      </button>
    </div>
  );
};

interface NotificationContainerProps {
  notifications: Array<{
    id: string;
    message: string;
    type: 'success' | 'error' | 'warning' | 'info';
  }>;
  onRemove: (id: string) => void;
}

export const NotificationContainer: React.FC<NotificationContainerProps> = ({
  notifications,
  onRemove
}) => {
  React.useEffect(() => {
    if (notifications.length > 0) {
      LoggerService.debug(
        `${notifications.length} notificação(ões) no container`,
        'NOTIFICATION_CONTAINER',
        notifications
      );
    }
  }, [notifications]);

  return (
    <div style={{
      position: 'fixed',
      top: '1rem',
      right: '1rem',
      zIndex: 9999,
      maxWidth: '400px',
      maxHeight: '90vh',
      overflowY: 'auto'
    }}>
      {notifications.map(notification => (
        <Notification
          key={notification.id}
          {...notification}
          onClose={onRemove}
        />
      ))}
    </div>
  );
};

export default Notification;
