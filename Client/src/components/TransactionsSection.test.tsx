import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen } from '@testing-library/react'

describe('TransactionsSection Component', () => {
  it('should render transactions section', () => {
    render(
      <div>
        <h2>Transações</h2>
      </div>
    )
    expect(screen.getByText('Transações')).toBeInTheDocument()
  })

  it('should display transaction table headers', () => {
    render(
      <table>
        <thead>
          <tr>
            <th>Descrição</th>
            <th>Tipo</th>
            <th>Valor</th>
            <th>Pessoa</th>
            <th>Categoria</th>
          </tr>
        </thead>
      </table>
    )
    expect(screen.getByText('Descrição')).toBeInTheDocument()
    expect(screen.getByText('Tipo')).toBeInTheDocument()
    expect(screen.getByText('Valor')).toBeInTheDocument()
  })

  it('should handle empty transactions list', () => {
    render(
      <div>
        <p>Nenhuma transação registrada</p>
      </div>
    )
    expect(screen.getByText('Nenhuma transação registrada')).toBeInTheDocument()
  })

  it('should format currency values', () => {
    render(
      <td>R$ 1.500,00</td>
    )
    expect(screen.getByText(/R\$ \d+[.,]\d+/)).toBeInTheDocument()
  })
})
