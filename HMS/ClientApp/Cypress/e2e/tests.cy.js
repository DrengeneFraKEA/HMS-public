/// <reference types="cypress" />

describe('HMS Basic Business Flow', () => {
    before(() => {
        cy.visit('http://localhost:44402');
    })

    it('should try to login', () => {

        var username = "1010101010";
        var password = "1234";

        cy.get('input[id="username"]').type(username);
        cy.get('input[id="password"]').type(password);

        // Click login and go to /main
        cy.get('button[id="loginbutton"]').click();

        cy.request('POST', 'http://localhost:44402/login', { username: username, password: password }).then(({ body }) => {
            expect(body.UserId).eq(31); // User logged in correctly?
            expect(body.Token).to.not.equal(""); // Is token legitimate?
            expect(body.Role).eq("doctor"); // Is there a role assigned?
        });

        cy.wait(3000);

        // Navigate to /appointments
        cy.get('a[id="appointments"]').click();
        cy.wait(3000);

        cy.get('span[id="appointmentStart"]').first().invoke('text').as('oldStart'); // Save old state of appointment start.
        cy.get('span[id="appointmentEnd"]').first().invoke('text').as('oldEnd'); // Save old state of appointment end.

        // Move into the scope of the old start and old end.
        cy.get('@oldStart').then((oldStart) => {
            cy.get('@oldEnd').then((oldEnd) => {

                var newStart = "2024-01-10T10:30";
                var newEnd = "2024-01-10T11:30";

                cy.get('button').first().click(); // opdater button
                cy.get('input').first().click().type(newStart); // start date of an appointment
                cy.wait(2000);
                cy.get('input').last().click().type(newEnd); // end date of an appointment
                cy.wait(2000);
                cy.get('button').first().click();

                // Refresh
                cy.get('a[id="home"]').click();
                cy.get('a[id="appointments"]').click();

                cy.wait(2000);

                // Now compare the OLD and new START dates of the appointment.
                expect(newStart).to.not.equal(oldStart); // Is the old appointment START date different from new start date?
                expect(newEnd).to.not.equal(oldEnd); // Is the old appointment END date different from new start date?

                // While we're at it, change it back.
                cy.get('button').first().click(); // opdater button
                cy.get('input').first().click().type(oldStart); // start date of an appointment
                cy.get('input').last().click().type(oldEnd); // end date of an appointment
                cy.get('button').first().click();
            });
        });

        cy.wait(2000);

        // Navigate to Prescriptions
        cy.get('a[id="prescriptions"]').click();
        cy.wait(2000);

        var realInput = "Cymbalta";
        var fakeInput = "Beer";

        // Type in real input.
        cy.get('input[class=search-input]').type(realInput);
        cy.get('button[class=search-button]').click();

        expect(cy.get('li[class=result-item]').should('have.length.at.least', 1)); // Expect there to be atleast 1 result.
        cy.wait(2000);

        // Type in fake input.
        cy.get('input[class=search-input]').clear();
        cy.get('input[class=search-input]').type(fakeInput);
        cy.get('button[class=search-button]').click();

        expect(cy.get('li[class=result-item]').should('have.length', 0)); // Expect there to be no results.
    })
})