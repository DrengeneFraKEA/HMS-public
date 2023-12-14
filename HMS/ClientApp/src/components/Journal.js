import React, { Component } from 'react';
import axios from 'axios';
import Navbar from './Navbar';
import '../styling/Journal.css';
import '../styling/navbar.css';

export class Journal extends Component {
    constructor(props) {
        super(props);

        this.state = {
            journals: [],
            newJournalNote: '',
            searchQuery: '',
            searchResult: null,
            editingJournals: {},
        };

        this.handleEditingNoteChange = this.handleEditingNoteChange.bind(this);
    }

    componentDidMount() {
        this.fetchJournals();
        this.setState({ searchResult: 0 });
    }

    fetchJournals = async () => {
        try {
            var request = "journal/doctor/" + localStorage.getItem('userid');

            const response = await axios.get(request, {
                headers: { Authorization: localStorage.getItem('token') },
            });

            this.setState({
                journals: response.data,
            });
        } catch (error) {
            console.error('Error fetching journals:', error);
        }
    };

    handleNoteChange = (e) => {
        // adding a new journal
        this.setState({
            newJournalNote: e.target.value,
        });
    };

    handleEditingNoteChange = (journalId, e) => {
        // Updating a journal.
        const { editingJournals } = this.state;

        this.setState({
            editingJournals: {
                ...editingJournals,
                [journalId]: {
                    ...editingJournals[journalId],
                    note: e.target.value,
                },
            },
        });
    };

    getJournalById = (journalId) => {
        const { journals } = this.state;
        return journals.find((journal) => journal.Id === journalId);
    };

    handleEditClick = (journalId) => {
        const { editingJournals } = this.state;

        // Set the journal as being edited
        this.setState({
            editingJournals: {
                ...editingJournals,
                [journalId]: {
                    editing: true,
                    note: this.getJournalById(journalId).Note, // Save the original note
                },
            },
        });
    };

    handleSaveClick = async (journalId) => {
        const { editingJournals } = this.state;
        const updatedNote = editingJournals[journalId]?.note || '';

        try {
            var request = "journal/journalid/" + journalId + "/newjournaltext/" + updatedNote;

            await axios.get(request, {
                headers: { Authorization: localStorage.getItem('token') },
            });
        } catch (error) {
            console.error('Error updating journals:', error);
        }

        console.log(updatedNote);

        // Clear the newJournalNote to reset the text area
        this.setState({
            newJournalNote: '',
        });

        this.setState((prevState) => ({
            editingJournals: {
                ...prevState.editingJournals,
                [journalId]: undefined, // Set to undefined to remove from the editing state
            },
        }));

        // Fetch updated journals
        this.fetchJournals();
    };

    handleAddJournal = async () => {
        const { newJournalNote } = this.state;
        const { searchQuery } = this.state;

        if (newJournalNote === "") return;

        var request = "journal/text/" + newJournalNote + "/cpr/" + searchQuery + "/doctor/" + localStorage.getItem("userid");

        try {
                await axios.get(request, {
                    headers: { Authorization: localStorage.getItem('token') }
            });
            this.fetchJournals(); // Refresh the list of journals after adding a new one

            // Clear the input field
            this.setState({
                newJournalNote: "",
                searchQuery: "",
                searchResult: 0
            });
        } catch (error) {
            console.error('Error adding journal:', error);
        }
    };

    handleDeleteJournal = async (journalId) => {
        try {
            await axios.delete(`journal/${journalId}`, {
                headers: { Authorization: localStorage.getItem('token') },
            });

            this.fetchJournals(); // Refresh the list of journals after deleting one
        } catch (error) {
            console.error('Error deleting journal:', error);
        }
    };

    handleSearchClick = async () => {
        const { searchQuery } = this.state;
        var request = "person/person/" + searchQuery;

        try {
            const response = await axios.get(request, {
                headers: { Authorization: localStorage.getItem('token') },
            });

            this.setState({searchResult: response.data});

        } catch (error) {
            console.error('Error deleting journal:', error);
        }
    };

    render() {
        const { journals, newJournalNote, searchQuery, searchResult, editingJournals } = this.state;

        return (
            <>
                <Navbar />
                <div className="journal-container">
                    <div className="add-journal-box">
                        <textarea
                            value={newJournalNote}
                            placeholder="Skriv her.."
                            onChange={this.handleNoteChange}
                        />
                        <button onClick={this.handleSearchClick}>Tilknyt patient</button>

                        <input
                            type="text"
                            placeholder="Indtast cpr"
                            value={searchQuery}
                            onChange={(e) => this.setState({ searchQuery: e.target.value })}
                        />

                        {searchResult !== null && (
                            <label className={searchResult ? 'success-label' : 'error-label'}>
                                {searchResult === 0
                                    ? ''
                                    : searchResult === true
                                    ? 'Patient tilknyttet'
                                    : 'Patient findes ikke'}
                            </label>
                        )}
                        <div style={{ marginTop: '10px' }}></div>
                        <button
                            onClick={this.handleAddJournal}
                            disabled={searchResult === 0 ? true : !searchResult}
                            className="add-journal-button"
                        >
                            Opret journal
                        </button>
                    </div>

                    <div className="journals-list">
                        <h2>Mine journaler</h2>
                        <ul>
                            {journals.map((journal) => (
                                <li key={journal.Id}>
                                    {editingJournals[journal.Id]?.editing && (
                                        <>
                                            <textarea
                                                value={editingJournals[journal.Id].note}
                                                onChange={(e) => this.handleEditingNoteChange(journal.Id, e)}
                                            />
                                            <button onClick={() => this.handleSaveClick(journal.Id)}>
                                                Gem
                                            </button>
                                        </>
                                    )}
                                        <>
                                            <div className="journal-note">{journal.Note}</div>
                                            <div className="journal-date">CPR: {journal.CPR}</div>
                                            <div className="journal-date">
                                                Created on: {journal.CreatedOn}
                                            </div>
                                            <div className="journal-date">
                                                Modified on: {journal.ModifiedOn}
                                            </div>
                                            <button onClick={() => this.handleEditClick(journal.Id)}>
                                                Opdater
                                            </button>
                                            <span className="button-spacing" />
                                            <button
                                                onClick={() => this.handleDeleteJournal(journal.Id)}
                                            >
                                                Slet
                                            </button>
                                        </>
                                </li>
                            ))}
                        </ul>
                    </div>
                </div>
            </>
        );
    }
}