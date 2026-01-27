import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import Header from '@/components/Header'

describe('Header Component', () => {
  it('should render header with title', () => {
    render(<Header />)
    const heading = screen.getByRole('heading')
    expect(heading).toBeInTheDocument()
  })

  it('should have logo text', () => {
    render(<Header />)
    const header = screen.getByRole('banner', { hidden: true }) || document.querySelector('header')
    expect(header).toBeInTheDocument()
  })

  it('should contain proper header structure', () => {
    const { container } = render(<Header />)
    const headerElement = container.querySelector('header')
    expect(headerElement).toBeInTheDocument()
  })

  it('should display header description', () => {
    render(<Header />)
    const description = screen.getByText(/Sistema completo/)
    expect(description).toBeInTheDocument()
  })
})
