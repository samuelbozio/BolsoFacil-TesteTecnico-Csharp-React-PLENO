/**
 * Componente de Seção de Categorias
 * 
 * Gerencia categorias de transações com:
 * - Listagem de todas as categorias
 * - Criação de novas categorias
 * - Filtro por tipo de finalidade (Despesa, Receita, Ambos)
 * - Exibição de totais por categoria
 * - Resumo total geral
 */

import React, { useState, useMemo } from 'react';
import { CategoryDTO, Category, PurposeType, TransactionType } from '../types';
import { useAsync, useMutation, useForm, useNotification, useModal } from '../hooks/useCustomHooks';
import apiService from '../services/apiService';
import { NotificationContainer } from './Notification';

const CategoriesSection: React.FC = () => {
  const { data: categories, loading, refetch } = useAsync(() => apiService.getCategories());
  const { data: categoriesWithTotals } = useAsync(() => apiService.getCategoriesWithTotals());
  const { mutate: createCategory } = useMutation(apiService.createCategory);

  const { notifications, notify, removeNotification } = useNotification();
  const { isOpen: isFormOpen, open: openForm, close: closeForm } = useModal();
  const [purposeFilter, setPurposeFilter] = useState<'all' | PurposeType>('all');

  const form = useForm({ description: '', purpose: PurposeType.Both });

  const purposeLabels = {
    [PurposeType.Expense]: 'Apenas Despesas',
    [PurposeType.Income]: 'Apenas Receitas',
    [PurposeType.Both]: 'Receitas e Despesas'
  };

  const filteredCategories = useMemo(() => {
    if (!categoriesWithTotals) return [];
    if (purposeFilter === 'all') return categoriesWithTotals;
    return categoriesWithTotals.filter(cat => cat.purpose === purposeFilter);
  }, [categoriesWithTotals, purposeFilter]);

  const totals = useMemo(() => {
    if (!filteredCategories) {
      return { totalIncome: 0, totalExpense: 0, balance: 0 };
    }
    return {
      totalIncome: filteredCategories.reduce((sum, cat) => sum + cat.totalIncome, 0),
      totalExpense: filteredCategories.reduce((sum, cat) => sum + cat.totalExpense, 0),
      balance: filteredCategories.reduce((sum, cat) => sum + cat.balance, 0)
    };
  }, [filteredCategories]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!form.values.description.trim()) {
      notify('Por favor, preencha a descrição', 'warning');
      return;
    }

    try {
      await createCategory(form.values);
      notify('Categoria criada com sucesso!', 'success');
      form.reset();
      closeForm();
      refetch();
    } catch (err: any) {
      notify(err.message || 'Erro ao criar categoria', 'error');
    }
  };

  if (loading) {
    return <div className="loading"><div className="spinner"></div></div>;
  }

  return (
    <div>
      <NotificationContainer notifications={notifications} onRemove={removeNotification} />

      <div className="card">
        <div className="flex flex-between">
          <h2>Gerenciamento de Categorias</h2>
          <button className="btn-primary" onClick={openForm}>+ Adicionar Categoria</button>
        </div>
      </div>

      {/* Modal de Formulário */}
      {isFormOpen && (
        <div className="modal-overlay">
          <div className="modal">
            <div className="modal-header">
              <h3>Adicionar Categoria</h3>
              <button className="close-btn" onClick={closeForm}>×</button>
            </div>

            <form onSubmit={handleSubmit}>
              <div className="modal-body">
                <div className="form-group">
                  <label htmlFor="description">Descrição *</label>
                  <input
                    id="description"
                    name="description"
                    type="text"
                    value={form.values.description}
                    onChange={form.handleChange}
                    placeholder="Ex: Alimentação"
                    maxLength={400}
                    required
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="purpose">Finalidade *</label>
                  <select
                    id="purpose"
                    name="purpose"
                    value={form.values.purpose}
                    onChange={(e) => form.setValue('purpose', Number(e.target.value) as PurposeType)}
                    required
                  >
                    <option value={PurposeType.Expense}>{purposeLabels[PurposeType.Expense]}</option>
                    <option value={PurposeType.Income}>{purposeLabels[PurposeType.Income]}</option>
                    <option value={PurposeType.Both}>{purposeLabels[PurposeType.Both]}</option>
                  </select>
                </div>

                <div style={{ fontSize: '0.875rem', color: '#666', marginTop: '1rem' }}>
                  <strong>Dica:</strong> A finalidade determina que tipo de transação pode usar esta categoria.
                  Por exemplo, uma categoria de "Salário" deve ter finalidade "Apenas Receitas".
                </div>
              </div>

              <div className="modal-footer">
                <button type="button" className="btn-secondary" onClick={closeForm}>
                  Cancelar
                </button>
                <button type="submit" className="btn-primary">
                  Criar
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Filtros */}
      {categories && categories.length > 0 && (
        <div className="card mb-3">
          <label style={{ marginBottom: '0.5rem', display: 'block', fontWeight: '500' }}>
            Filtrar por finalidade:
          </label>
          <div className="flex" style={{ gap: '0.5rem', flexWrap: 'wrap' }}>
            <button
              className={`btn-secondary ${purposeFilter === 'all' ? 'btn-primary' : ''}`}
              onClick={() => setPurposeFilter('all')}
              style={{
                backgroundColor: purposeFilter === 'all' ? '#2563eb' : 'var(--light-bg)',
                color: purposeFilter === 'all' ? 'white' : 'var(--text-primary)'
              }}
            >
              Todas
            </button>
            {Object.entries(purposeLabels).map(([key, label]) => (
              <button
                key={key}
                className={`btn-secondary`}
                onClick={() => setPurposeFilter(Number(key) as PurposeType)}
                style={{
                  backgroundColor: purposeFilter === Number(key) ? '#2563eb' : 'var(--light-bg)',
                  color: purposeFilter === Number(key) ? 'white' : 'var(--text-primary)'
                }}
              >
                {label}
              </button>
            ))}
          </div>
        </div>
      )}

      {/* Listagem */}
      {filteredCategories && filteredCategories.length > 0 ? (
        <>
          <div style={{ overflowX: 'auto' }}>
            <table>
              <thead>
                <tr>
                  <th>Descrição</th>
                  <th>Finalidade</th>
                  <th className="text-right">Receitas</th>
                  <th className="text-right">Despesas</th>
                  <th className="text-right">Saldo</th>
                </tr>
              </thead>
              <tbody>
                {filteredCategories.map(category => (
                  <tr key={category.id}>
                    <td><strong>{category.description}</strong></td>
                    <td>{purposeLabels[category.purpose]}</td>
                    <td className="text-right text-success">R$ {category.totalIncome.toFixed(2)}</td>
                    <td className="text-right text-danger">R$ {category.totalExpense.toFixed(2)}</td>
                    <td className="text-right">
                      <span className={category.balance >= 0 ? 'text-success' : 'text-danger'}>
                        R$ {category.balance.toFixed(2)}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Totais Filtrados */}
          <div className="card mt-4" style={{ backgroundColor: '#f0f9ff' }}>
            <h3>Total {purposeFilter !== 'all' ? `(${purposeLabels[purposeFilter as PurposeType]})` : 'Geral'}</h3>
            <div className="grid grid-3 mt-3">
              <div style={{ textAlign: 'center', padding: '1rem' }}>
                <div style={{ fontSize: '0.875rem', color: '#666', marginBottom: '0.5rem' }}>
                  Receitas
                </div>
                <div style={{ fontSize: '1.5rem', fontWeight: '600', color: '#10b981' }}>
                  R$ {totals.totalIncome.toFixed(2)}
                </div>
              </div>
              <div style={{ textAlign: 'center', padding: '1rem' }}>
                <div style={{ fontSize: '0.875rem', color: '#666', marginBottom: '0.5rem' }}>
                  Despesas
                </div>
                <div style={{ fontSize: '1.5rem', fontWeight: '600', color: '#ef4444' }}>
                  R$ {totals.totalExpense.toFixed(2)}
                </div>
              </div>
              <div style={{ textAlign: 'center', padding: '1rem' }}>
                <div style={{ fontSize: '0.875rem', color: '#666', marginBottom: '0.5rem' }}>
                  Saldo
                </div>
                <div style={{
                  fontSize: '1.5rem',
                  fontWeight: '600',
                  color: totals.balance >= 0 ? '#10b981' : '#ef4444'
                }}>
                  R$ {totals.balance.toFixed(2)}
                </div>
              </div>
            </div>
          </div>
        </>
      ) : (
        <div className="card">
          <div className="empty-state">
            <h3>Nenhuma categoria encontrada</h3>
            <p>Clique em "Adicionar Categoria" para começar</p>
          </div>
        </div>
      )}
    </div>
  );
};

export default CategoriesSection;
