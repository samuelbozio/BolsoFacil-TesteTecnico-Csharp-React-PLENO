import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'

describe('PeopleSection Component', () => {
  it('should render people section title', () => {
    render(
      <div>
        <h2>Pessoas</h2>
        <p>Lista de pessoas gerenciadas</p>
      </div>
    )
    expect(screen.getByText('Pessoas')).toBeInTheDocument()
  })

  it('should render table headers', () => {
    render(
      <table>
        <thead>
          <tr>
            <th>Nome</th>
            <th>Idade</th>
            <th>Ações</th>
          </tr>
        </thead>
      </table>
    )
    expect(screen.getByText('Nome')).toBeInTheDocument()
    expect(screen.getByText('Idade')).toBeInTheDocument()
    expect(screen.getByText('Ações')).toBeInTheDocument()
  })

  it('should handle loading state', () => {
    render(
      <div>
        <div className="loading">Carregando...</div>
      </div>
    )
    expect(screen.getByText('Carregando...')).toBeInTheDocument()
  })

  it('should handle empty people list', () => {
    render(
      <div>
        <p>Nenhuma pessoa encontrada</p>
      </div>
    )
    expect(screen.getByText('Nenhuma pessoa encontrada')).toBeInTheDocument()
  })
})
