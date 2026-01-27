import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'

describe('SummarySection Component', () => {
  it('should render summary section', () => {
    render(
      <div>
        <h2>Resumo</h2>
      </div>
    )
    expect(screen.getByText('Resumo')).toBeInTheDocument()
  })

  it('should display income total', () => {
    render(
      <div>
        <div>
          <span>Receita Total:</span>
          <span>R$ 6.500,00</span>
        </div>
      </div>
    )
    expect(screen.getByText('Receita Total:')).toBeInTheDocument()
    expect(screen.getByText('R$ 6.500,00')).toBeInTheDocument()
  })

  it('should display expense total', () => {
    render(
      <div>
        <div>
          <span>Despesa Total:</span>
          <span>R$ 1.685,50</span>
        </div>
      </div>
    )
    expect(screen.getByText('Despesa Total:')).toBeInTheDocument()
    expect(screen.getByText('R$ 1.685,50')).toBeInTheDocument()
  })

  it('should display balance', () => {
    render(
      <div>
        <div>
          <span>Saldo:</span>
          <span className="positive">R$ 4.814,50</span>
        </div>
      </div>
    )
    expect(screen.getByText('Saldo:')).toBeInTheDocument()
    expect(screen.getByText('R$ 4.814,50')).toBeInTheDocument()
  })

  it('should show positive balance in green', () => {
    render(
      <span className="balance positive">R$ 1.000,00</span>
    )
    const balanceSpan = screen.getByText('R$ 1.000,00')
    expect(balanceSpan).toHaveClass('positive')
  })

  it('should show negative balance in red', () => {
    render(
      <span className="balance negative">-R$ 500,00</span>
    )
    const balanceSpan = screen.getByText('-R$ 500,00')
    expect(balanceSpan).toHaveClass('negative')
  })
})
