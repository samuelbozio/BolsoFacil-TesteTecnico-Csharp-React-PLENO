import React from 'react';
import { PurposeType } from '../types';
import { useAsync } from '../hooks/useCustomHooks';
import apiService from '../services/apiService';

const SummarySection: React.FC = () => {
  const { data: people, loading: peopleLoading } = useAsync(() => apiService.getPeople());
  const { data: categoriesWithTotals, loading: categoriesLoading } = useAsync(() => apiService.getCategoriesWithTotals());
  const { data: summary, loading: summaryLoading } = useAsync(() => apiService.getPeopleTotalSummary());

  const purposeLabels = {
    [PurposeType.Expense]: 'Despesas',
    [PurposeType.Income]: 'Receitas',
    [PurposeType.Both]: 'Ambas'
  };

  const loading = peopleLoading || categoriesLoading || summaryLoading;

  if (loading) {
    return <div className="loading"><div className="spinner"></div></div>;
  }

  return (
    <div>
      {summary && (
        <div className="card" style={{ backgroundColor: '#f0f9ff', borderLeft: '4px solid #2563eb' }}>
          <h2 style={{ marginTop: 0 }}>Resumo Geral</h2>
          <div className="grid grid-3 mt-4">
            <div className="summary-card" style={{ padding: '1.5rem', textAlign: 'center' }}>
              <div style={{ fontSize: '0.875rem', color: '#666', marginBottom: '0.75rem' }}>
                TOTAL DE RECEITAS
              </div>
              <div style={{ fontSize: '2rem', fontWeight: '700', color: '#10b981', marginBottom: '0.5rem' }}>
                R$ {(summary.totalIncome ?? 0).toFixed(2)}
              </div>
              <div style={{ fontSize: '0.75rem', color: '#999' }}>
                Todas as receitas do sistema
              </div>
            </div>

            <div className="summary-card" style={{ padding: '1.5rem', textAlign: 'center' }}>
              <div style={{ fontSize: '0.875rem', color: '#666', marginBottom: '0.75rem' }}>
                TOTAL DE DESPESAS
              </div>
              <div style={{ fontSize: '2rem', fontWeight: '700', color: '#ef4444', marginBottom: '0.5rem' }}>
                R$ {(summary.totalExpense ?? 0).toFixed(2)}
              </div>
              <div style={{ fontSize: '0.75rem', color: '#999' }}>
                Todas as despesas do sistema
              </div>
            </div>

            <div className="summary-card" style={{
              padding: '1.5rem',
              textAlign: 'center',
              backgroundColor: (summary.netBalance ?? 0) >= 0 ? '#dcfce7' : '#fee2e2',
              borderRadius: '0.5rem'
            }}>
              <div style={{ fontSize: '0.875rem', color: '#666', marginBottom: '0.75rem' }}>
                SALDO LÍQUIDO
              </div>
              <div style={{
                fontSize: '2rem',
                fontWeight: '700',
                color: (summary.netBalance ?? 0) >= 0 ? '#16a34a' : '#dc2626',
                marginBottom: '0.5rem'
              }}>
                R$ {(summary.netBalance ?? 0).toFixed(2)}
              </div>
              <div style={{ fontSize: '0.75rem', color: '#999' }}>
                {(summary.netBalance ?? 0) >= 0 ? 'Superávit' : 'Déficit'}
              </div>
            </div>
          </div>
        </div>
      )}

      {people && people.length > 0 && (
        <div className="card">
          <h2>Resumo por Pessoa</h2>
          <div style={{ overflowX: 'auto', marginTop: '1.5rem' }}>
            <table>
              <thead>
                <tr>
                  <th>Nome</th>
                  <th>Idade</th>
                  <th className="text-right">Receitas</th>
                  <th className="text-right">Despesas</th>
                  <th className="text-right">Saldo</th>
                </tr>
              </thead>
              <tbody>
                {people.map(person => (
                  <tr key={person.id} style={{
                    backgroundColor: (person.balance ?? 0) >= 0 ? 'rgba(16, 185, 129, 0.05)' : 'rgba(239, 68, 68, 0.05)'
                  }}>
                    <td><strong>{person.name}</strong></td>
                    <td>{person.age} anos</td>
                    <td className="text-right" style={{ color: '#10b981', fontWeight: '600' }}>
                      R$ {(person.totalIncome ?? 0).toFixed(2)}
                    </td>
                    <td className="text-right" style={{ color: '#ef4444', fontWeight: '600' }}>
                      R$ {(person.totalExpense ?? 0).toFixed(2)}
                    </td>
                    <td className="text-right" style={{ fontWeight: '700' }}>
                      <span style={{ color: (person.balance ?? 0) >= 0 ? '#16a34a' : '#dc2626' }}>
                        R$ {(person.balance ?? 0).toFixed(2)}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {categoriesWithTotals && categoriesWithTotals.length > 0 && (
        <div className="card">
          <h2>Resumo por Categoria</h2>
          <div style={{ overflowX: 'auto', marginTop: '1.5rem' }}>
            <table>
              <thead>
                <tr>
                  <th>Categoria</th>
                  <th>Propósito</th>
                  <th className="text-right">Total de Despesas</th>
                </tr>
              </thead>
              <tbody>
                {categoriesWithTotals.map(category => (
                  <tr key={category.id}>
                    <td><strong>{category.description}</strong></td>
                    <td>
                      <span style={{
                        display: 'inline-block',
                        padding: '0.25rem 0.75rem',
                        borderRadius: '0.25rem',
                        backgroundColor: '#e0e7ff',
                        color: '#4338ca',
                        fontSize: '0.875rem',
                        fontWeight: '500'
                      }}>
                        {purposeLabels[category.purpose]}
                      </span>
                    </td>
                    <td className="text-right" style={{ fontWeight: '600' }}>
                      R$ {(category.totalExpense ?? 0).toFixed(2)}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Estatísticas Adicionais */}
      {people && categoriesWithTotals && (
        <div className="card" style={{ backgroundColor: '#f3f4f6' }}>
          <h3>📈 Estatísticas Rápidas</h3>
          <div className="grid grid-3 mt-3">
            <div style={{ padding: '1rem' }}>
              <div style={{ fontSize: '0.875rem', color: '#666' }}>Total de Pessoas</div>
              <div style={{ fontSize: '1.75rem', fontWeight: '700', color: '#2563eb', marginTop: '0.5rem' }}>
                {people.length}
              </div>
            </div>
            <div style={{ padding: '1rem' }}>
              <div style={{ fontSize: '0.875rem', color: '#666' }}>Total de Categorias</div>
              <div style={{ fontSize: '1.75rem', fontWeight: '700', color: '#7c3aed', marginTop: '0.5rem' }}>
                {categoriesWithTotals.length}
              </div>
            </div>
            <div style={{ padding: '1rem' }}>
              <div style={{ fontSize: '0.875rem', color: '#666' }}>Razão Receita/Despesa</div>
              <div style={{
                fontSize: '1.75rem',
                fontWeight: '700',
                color: summary && summary.totalExpense > 0
                  ? (summary.totalIncome / summary.totalExpense >= 1 ? '#10b981' : '#ef4444')
                  : '#999',
                marginTop: '0.5rem'
              }}>
                {summary && summary.totalExpense > 0
                  ? (summary.totalIncome / summary.totalExpense).toFixed(2) + 'x'
                  : 'N/A'}
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Dica de Uso */}
      <div className="card" style={{ backgroundColor: '#fef3c7', borderLeft: '4px solid #f59e0b' }}>
        <h3 style={{ color: '#92400e', marginTop: 0 }}>💡 Dica</h3>
        <p style={{ color: '#92400e' }}>
          Acompanhe regularmente o seu fluxo de caixa. Uma razão receita/despesa abaixo de 1 indica
          que você está gastando mais do que ganhando. Tente aumentar as receitas ou reduzir as despesas.
        </p>
      </div>
    </div>
  );
};

export default SummarySection;
