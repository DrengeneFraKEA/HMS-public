import { Component } from "react";
import { Link } from 'react-router-dom';
import '../styling/navbar.css';

export default class Navbar extends Component
{
    render() {
        return (
            <nav className="navbar">
                <div className="navbar-container">
                    <Link to="/main" className="navbar-link">Hjem</Link>
                    <Link to="/appointments" className="navbar-link">Henvisninger</Link>
                    <Link to="/prescriptions" className="navbar-link">Recept</Link>
                    <Link to="/journal" className="navbar-link">Journaler</Link>
                    <Link to="/personalinfo" className="navbar-link">Personlig Information</Link>
                    <Link to="/rating" className="navbar-link">Rating</Link>
                </div>
            </nav>
        );
    }
}