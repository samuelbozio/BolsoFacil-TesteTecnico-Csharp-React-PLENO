import { useState } from 'react';
import Header from './components/Header';
import PeopleSection from './components/PeopleSection';
import CategoriesSection from './components/CategoriesSection';
import TransactionsSection from './components/TransactionsSection';
import SummarySection from './components/SummarySection';
import './styles/index.css';

type PageType = 'people' | 'categories' | 'transactions' | 'summary';

function App() {
  const [currentPage, setCurrentPage] = useState<PageType>('people');

  const renderContent = () => {
    switch (currentPage) {
      case 'people':
        return <PeopleSection />;
      case 'categories':
        return <CategoriesSection />;
      case 'transactions':
        return <TransactionsSection />;
      case 'summary':
        return <SummarySection />;
      default:
        return <PeopleSection />;
    }
  };

  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <Header currentPage={currentPage} onNavigate={setCurrentPage} />

      <main style={{ flex: 1, paddingBottom: '2rem' }}>
        <div className="container">
          {renderContent()}
        </div>
      </main>

      <footer style={{
        backgroundColor: '#f3f4f6',
        borderTop: '1px solid var(--border-color)',
        padding: '2rem 0',
        marginTop: 'auto'
      }}>
        <div className="container">
          <div style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))',
            gap: '2rem',
            marginBottom: '2rem'
          }}>
            <div>
              <h4>Sobre o Sistema</h4>
              <p style={{ color: 'var(--text-secondary)' }}>
                Sistema completo e responsivo para controle de gastos residenciais,
                desenvolvido com tecnologias modernas e boas práticas de desenvolvimento.
              </p>
            </div>

            <div>
              <h4>Tecnologias</h4>
              <ul style={{ listStyle: 'none', padding: 0 }}>
                <li>React 18 com TypeScript</li>
                <li>.NET Core 8.0 / 10.0</li>
                <li>Entity Framework Core</li>
                <li>SQLite para persistência</li>
              </ul>
            </div>

            <div>
              <h4>Funcionalidades Principais</h4>
              <ul style={{ listStyle: 'none', padding: 0 }}>
                <li>Gerenciar pessoas</li>
                <li>Criar categorias</li>
                <li>Registrar transações</li>
                <li>Visualizar relatórios</li>
              </ul>
            </div>
          </div>

          <div style={{
            borderTop: '1px solid var(--border-color)',
            paddingTop: '2rem',
            textAlign: 'center',
            color: 'var(--text-secondary)'
          }}>
            <p>
              © 2024 - Controle de Gastos Residenciais | 
              <span style={{ margin: '0 0.5rem' }}>Desenvolvido com amor</span>|
              <span style={{ margin: '0 0.5rem' }}>v1.0.0</span>
            </p>
          </div>
        </div>
      </footer>
    </div>
  );
}

export default App;
