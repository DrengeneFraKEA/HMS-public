import { Component } from "react";
import axios from 'axios';
import { Link } from 'react-router-dom';
import '../styling/navbar.css';

export default class Navbar extends Component
{
    constructor(props) {
        super(props);

        // Retrieve the stored value from localStorage
        const storedOption = localStorage.getItem('selectedOption');

        this.state = {
            selectedOption: storedOption || '',
        };
    }

    handleDropdownChange = (e) => {
        const selectedOption = e.target.value;

        // Save the selected value to localStorage
        localStorage.setItem('selectedOption', selectedOption);

        this.setState({
            selectedOption,
        });

        try {
            axios.post("database", { value: selectedOption });
        }
        catch (error) {
            console.log(error);
        }

    };

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

                <div className="dropdown">
                    <select value={this.state.selectedOption} onChange={this.handleDropdownChange}>
                        <option value="0">MySQL</option>
                        <option value="1">MongoDB</option>
                        <option value="2">GraphQL</option>
                    </select>
                </div>

            </nav>
        );
    }
}