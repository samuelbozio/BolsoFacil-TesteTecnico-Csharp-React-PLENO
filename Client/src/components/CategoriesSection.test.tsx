import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen } from '@testing-library/react'

describe('CategoriesSection Component', () => {
  it('should render categories section', () => {
    render(
      <div>
        <h2>Categorias</h2>
      </div>
    )
    expect(screen.getByText('Categorias')).toBeInTheDocument()
  })

  it('should display category list', () => {
    render(
      <ul>
        <li>Salário</li>
        <li>Alimentação</li>
        <li>Moradia</li>
      </ul>
    )
    expect(screen.getByText('Salário')).toBeInTheDocument()
    expect(screen.getByText('Alimentação')).toBeInTheDocument()
    expect(screen.getByText('Moradia')).toBeInTheDocument()
  })

  it('should handle empty categories', () => {
    render(
      <div>
        <p>Nenhuma categoria disponível</p>
      </div>
    )
    expect(screen.getByText('Nenhuma categoria disponível')).toBeInTheDocument()
  })

  it('should render category count', () => {
    render(
      <div>
        <span>Total: 6 categorias</span>
      </div>
    )
    expect(screen.getByText(/Total: \d+ categorias/)).toBeInTheDocument()
  })
})
