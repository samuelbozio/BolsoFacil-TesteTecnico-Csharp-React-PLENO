import { ReactNode } from 'react';

interface HeaderProps {
  currentPage?: 'people' | 'categories' | 'transactions' | 'summary';
  onNavigate?: (page: 'people' | 'categories' | 'transactions' | 'summary') => void;
}

const Header = ({ currentPage = 'people', onNavigate }: HeaderProps) => {
  return (
    <header className="header">
      <div className="container">
        <div>
          <h1>Controle de Gastos Residenciais</h1>
          <p>Sistema completo para gerenciar receitas e despesas da sua casa</p>
        </div>
        
        {onNavigate && (
          <nav style={{
            marginTop: '1.5rem',
            display: 'flex',
            gap: '0.75rem',
            flexWrap: 'wrap'
          }}>
            <NavigationLink
              label="Pessoas"
              active={currentPage === 'people'}
              onClick={() => onNavigate('people')}
            />
            <NavigationLink
              label="Categorias"
              active={currentPage === 'categories'}
              onClick={() => onNavigate('categories')}
            />
            <NavigationLink
              label="Transações"
              active={currentPage === 'transactions'}
              onClick={() => onNavigate('transactions')}
            />
            <NavigationLink
              label="Resumo"
              active={currentPage === 'summary'}
              onClick={() => onNavigate('summary')}
            />
          </nav>
        )}
      </div>
    </header>
  );
};

interface NavigationLinkProps {
  label: string;
  active: boolean;
  onClick: () => void;
}

const NavigationLink = ({ label, active, onClick }: NavigationLinkProps) => {
  return (
    <button
      onClick={onClick}
      style={{
        background: active ? 'rgba(255, 255, 255, 0.2)' : 'transparent',
        color: 'white',
        border: active ? '1.5px solid white' : '1.5px solid rgba(255, 255, 255, 0.3)',
        padding: '0.625rem 1.25rem',
        borderRadius: '0.5rem',
        cursor: 'pointer',
        fontSize: '0.9rem',
        fontWeight: active ? '600' : '500',
        transition: 'all 0.2s'
      }}
    >
      {label}
    </button>
  );
};

export default Header;
